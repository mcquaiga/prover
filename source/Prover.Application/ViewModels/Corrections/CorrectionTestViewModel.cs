using DynamicData;
using Prover.Application.Extensions;
using Prover.Calculations;
using Prover.Shared.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Prover.Application.ViewModels.Corrections
{
    public abstract class VerifyViewModel : ViewModelWithIdBase
    {

    }


    public abstract class VerificationViewModel : VerifyViewModel, IAssertVerification
    {

        protected VerificationViewModel()
        {
            VerifiedObservable = Observable.Empty(false);

            this.WhenAnyObservable(x => x.VerifiedObservable)
                .Throttle(TimeSpan.FromMilliseconds(25), RxApp.TaskpoolScheduler)
                .ToPropertyEx(this, x => x.Verified, deferSubscription: true, scheduler: RxApp.MainThreadScheduler);
        }

        //protected VerificationViewModel(bool verified)
        //{
        //    VerifiedObservable = Observable.Empty(verified);

        //    this.WhenAnyObservable(x => x.VerifiedObservable)
        //        .Throttle(TimeSpan.FromMilliseconds(25), RxApp.TaskpoolScheduler)
        //        .ToPropertyEx(this, x => x.Verified, verified, deferSubscription: true, scheduler: RxApp.MainThreadScheduler);
        //}

        public virtual extern bool Verified { [ObservableAsProperty] get; }

        public IObservable<bool> VerifiedObservable { get; protected set; }

        protected void RegisterVerificationsForVerified(ICollection<VerificationViewModel> verifications)
        {
            if (verifications == null || !verifications.Any())
                return;

            VerifiedObservable = verifications.AsObservableChangeSet()
                .AutoRefresh(model => model.Verified)
                .ToCollection()
                .Select(x => x.Any() && x.All(y => y != null && y.Verified))
                //.LogDebug(x => $"{GetType().Name} - Verified = {x}", Logger)
                .LogErrors(Logger)
                .LoggedCatch(this, VerifiedObservable)
                .ObserveOn(RxApp.MainThreadScheduler);
        }
    }

    public interface IAssertExpectedActual
    {
        decimal PassTolerance { get; }
        decimal ExpectedValue { get; }
        decimal ActualValue { get; }
        decimal PercentError { get; }
        bool Verified { get; }
    }

    public interface IDeviceItemsGroup<T>
        where T : class
    {
        T Items { get; set; }
    }

    public abstract class VarianceTestViewModel : VerificationViewModel, IAssertExpectedActual
    {
        protected VarianceTestViewModel()
        {
        }

        protected VarianceTestViewModel(decimal passTolerance)
        {
            PassTolerance = passTolerance;

            VerifiedObservable = this.WhenAnyValue(x => x.PercentError)
                .Select(p => p.IsBetween(PassTolerance));


            this.WhenAnyValue(x => x.ExpectedValue, x => x.ActualValue, Calculators.PercentDeviation)
                .ToPropertyEx(this, x => x.PercentError, 100m, true)
                .DisposeWith(Cleanup);
        }

        [Reactive] public decimal PassTolerance { get; protected set; }
        public extern decimal ExpectedValue { [ObservableAsProperty] get; }
        public extern decimal ActualValue { [ObservableAsProperty] get; }
        public extern decimal PercentError { [ObservableAsProperty] get; }
    }

    public abstract class DeviationTestViewModel<T> : ViewModelWithIdBase, IAssertVerification, IDeviceItemsGroup<T>
        where T : class
    {
        protected DeviationTestViewModel()
        {
        }

        protected DeviationTestViewModel(int passTolerance)
        {
            PassTolerance = passTolerance;

            this.WhenAnyValue(x => x.ExpectedValue, x => x.ActualValue, Calculators.Deviation)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(x => ActualValue == 0 && ExpectedValue == 0 ? 100 : x)
                .ToPropertyEx(this, x => x.Deviation, 100)
                .DisposeWith(Cleanup);

            this.WhenAnyValue(x => x.Deviation)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(p => p.IsBetween(PassTolerance))
                .ToPropertyEx(this, x => x.Verified)
                .DisposeWith(Cleanup);
        }

        [Reactive] public T Items { get; set; }
        [Reactive] public int PassTolerance { get; protected set; }
        [Reactive] public int ActualValue { get; set; }
        public extern int ExpectedValue { [ObservableAsProperty] get; }
        public extern int Deviation { [ObservableAsProperty] get; }
        public extern bool Verified { [ObservableAsProperty] get; }
    }

    public abstract class ItemVarianceTestViewModel<T> : VarianceTestViewModel, IDeviceItemsGroup<T>
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
        protected readonly ReactiveCommand<Unit, decimal> UpdateFactor;

        protected CorrectionTestViewModel()
        {
        }

        protected CorrectionTestViewModel(T items, decimal passTolerance) : base(items, passTolerance)
        {
            UpdateFactor = ReactiveCommand.Create<Unit, decimal>(_ => CalculatorFactory.Invoke().CalculateFactor());

            UpdateFactor
                    .ToPropertyEx(this, x => x.ExpectedValue)
                .DisposeWith(Cleanup);

            UpdateFactor.DisposeWith(Cleanup);
        }

        protected abstract Func<ICorrectionCalculator> CalculatorFactory { get; }

        //public IItemGroup TestItems { get; set; }
    }

}