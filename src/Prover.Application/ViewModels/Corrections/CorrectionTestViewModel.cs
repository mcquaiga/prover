using System;
using System.Reactive;
using System.Reactive.Linq;
using Core.GasCalculations;
using DynamicData.Binding;
using Prover.Shared.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels.Corrections
{
    public interface ISetVerified : IAssertVerification
    {
        void SetVerified(bool verified);
    }

    public abstract class VerificationViewModel : BaseViewModel, IAssertVerification
    {
        private readonly ReactiveCommand<bool, bool> _setVerified;

        protected VerificationViewModel()
        {
            _setVerified = ReactiveCommand.Create<bool, bool>(v => v);
            _setVerified.ToPropertyEx(this, x => x.Verified);

            //this.WhenAnyValue(x => x.IsVerified)
            //    .ToPropertyEx(this, x => x.Verified);
        }

        //[Reactive] private bool IsVerified { get; set; }

        public extern bool Verified { [ObservableAsProperty] get; }

        public void SetVerified(bool verified)
        {
            verified = SetOverrideVerify(verified);
            _setVerified.Execute(verified);
        }

        protected virtual bool SetOverrideVerify(bool verified) => verified;
    }

    public interface IAssertExpectedActual
    {
        decimal PassTolerance { get; }
        decimal ExpectedValue { get; }
        decimal ActualValue { get; }
        decimal PercentError { get; }
        bool Verified { get; }
    }

    public abstract class VarianceTestViewModel : VerificationViewModel, IAssertExpectedActual
    {
        protected VarianceTestViewModel()
        {
        }

        protected VarianceTestViewModel(decimal passTolerance)
        {
            PassTolerance = passTolerance;

            this.WhenAnyValue(x => x.PercentError)
                .Select(p => p.IsBetween(PassTolerance))
                .ToPropertyEx(this, x => x.Verified, deferSubscription: true);

            this.WhenAnyValue(x => x.ExpectedValue, x => x.ActualValue, Calculators.PercentDeviation)
                .ToPropertyEx(this, x => x.PercentError, 100m, true);
        }

        [Reactive] public decimal PassTolerance { get; protected set; }
        public extern decimal ExpectedValue { [ObservableAsProperty] get; }
        public extern decimal ActualValue { [ObservableAsProperty] get; }

        public extern decimal PercentError { [ObservableAsProperty] get; }
    }

    public abstract class DeviationTestViewModel<T> : BaseViewModel, IAssertVerification
        where T : class
    {
        protected DeviationTestViewModel()
        {
        }

        protected DeviationTestViewModel(int passTolerance)
        {
            PassTolerance = passTolerance;

            this.WhenAnyValue(x => x.ExpectedValue, x => x.ActualValue, Calculators.Deviation)
                .ToPropertyEx(this, x => x.Deviation, 100, true);

            this.WhenAnyValue(x => x.Deviation)
                .Select(p => p.IsBetween(PassTolerance))
                .ToPropertyEx(this, x => x.Verified, deferSubscription: true);
        }

        [Reactive] public T Items { get; set; }
        [Reactive] public int PassTolerance { get; protected set; }
        [Reactive] public int ExpectedValue { get; set; }
        public extern int ActualValue { [ObservableAsProperty] get; }
        public extern int Deviation { [ObservableAsProperty] get; }
        public extern bool Verified { [ObservableAsProperty] get; }
    }

    public abstract class ItemVarianceTestViewModel<T> : VarianceTestViewModel
        where T : class
    {
        protected ItemVarianceTestViewModel(T items, decimal passTolerance) : base(passTolerance) => Items = items;

        protected ItemVarianceTestViewModel()
        {
        }

        [Reactive] public T Items { get; set; }
    }


    public abstract class CorrectionTestViewModel<T> : ItemVarianceTestViewModel<T>
        where T : class
    {
        protected ReactiveCommand<Unit, decimal> UpdateFactor;

        protected CorrectionTestViewModel()
        {
        }

        protected CorrectionTestViewModel(T items, decimal passTolerance) : base(items, passTolerance)
        {
            UpdateFactor = ReactiveCommand.Create<Unit, decimal>(_ => CalculatorFactory.Invoke().CalculateFactor());

            UpdateFactor.ToPropertyEx(this, x => x.ExpectedValue);
        }

        protected abstract Func<ICorrectionCalculator> CalculatorFactory { get; }

        //public IItemGroup TestItems { get; set; }
    }
}