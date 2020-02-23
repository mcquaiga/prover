using System;
using System.Reactive;
using System.Reactive.Linq;
using Core.GasCalculations;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shared.Domain;
using Shared.Extensions;

namespace Application.ViewModels.Corrections
{
    public interface IVerify
    {
        bool Verified { get; }
        void SetVerified(bool verified);
    }

    public abstract class VerificationTestProxy : AbstractNotifyPropertyChanged, IVerify
    {
        public virtual bool Verified { get; }
        public void SetVerified(bool verified)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class VerificationViewModel : BaseViewModel, IVerify
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
            _setVerified.Execute(verified);
        }
    }

    //public abstract class VarianceTestProxy : AbstractNotifyPropertyChanged, IVerify
    //{
    //    public IProperty<decimal> PassTolerance { get; protected set; }
    //    public virtual decimal ExpectedValue { get; protected set; }
    //    public virtual decimal ActualValue { get; protected set; }

    //    public virtual decimal PercentError { get; protected set; }
    //    public virtual bool Verified { get; }
    //    public void SetVerified(bool verified)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public abstract class VarianceTestViewModelTest : ReactiveObject
    //{
    //    public VarianceTestProxy Proxy { get; }

    //    protected VarianceTestViewModelTest(VarianceTestProxy proxy)
    //    {
    //        Proxy = proxy;

    //        this.WhenAnyValue(x => x.Proxy.PercentError)
    //            .Select(p => p.IsBetween(Proxy.PassTolerance))
    //            .ToPropertyEx(this, x => x.Proxy.Verified, deferSubscription: true);

    //        this.WhenAnyValue(x => x.Proxy.ExpectedValue, x => x.Proxy.ActualValue, Calculators.PercentDeviation)
    //            .ToPropertyEx(this, x => x.PercentError, 100m, true);
    //    }
    //}

    public abstract class VarianceTestViewModel : VerificationViewModel
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