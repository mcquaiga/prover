using System;
using System.Reactive;
using System.Reactive.Linq;
using Core.GasCalculations;
using Devices.Core.Items.ItemGroups;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shared.Extensions;

namespace Application.ViewModels
{
    public abstract class BaseViewModel : ReactiveObject
    {
        protected BaseViewModel()
        {
            
        }

        public Guid Id { get; protected set; }
    }


    public abstract class CorrectionTestViewModel<T> : BaseViewModel
        where T : ItemGroup
    {
        protected ReactiveCommand<Unit, decimal> UpdateCommand;

        protected CorrectionTestViewModel(){ }

        protected CorrectionTestViewModel(T items, decimal passTolerance)
        {
            Items = items;
            PassTolerance = passTolerance;

            UpdateCommand = ReactiveCommand.Create<Unit, decimal>(_ => CalculatorFactory.Invoke().CalculateFactor());
            UpdateCommand
                .ToPropertyEx(this, x => x.ExpectedValue);

            //this.WhenPropertyChanged(t => t.Items, false)
            //    .Select(x => Unit.Default)
            //    .InvokeCommand(UpdateCommand);

            this.WhenAnyValue(x => x.PercentError)
                .Select(p => p.IsBetween(PassTolerance))
                .ToPropertyEx(this, x => x.Verified, deferSubscription: true);

            this.WhenAnyValue(x => x.ExpectedValue, x => x.ActualValue, Calculators.PercentDeviation)
                .ToPropertyEx(this, x => x.PercentError, 100m, deferSubscription: true);
        }

        #region Public Properties

        [Reactive] public T Items { get; set; }

        [Reactive] public decimal PassTolerance { get; protected set; }

        public extern decimal ExpectedValue { [ObservableAsProperty] get; }
        public extern decimal ActualValue { [ObservableAsProperty] get; }
        public extern decimal PercentError { [ObservableAsProperty] get; }
        public extern bool Verified { [ObservableAsProperty] get; }

        #endregion

        #region Protected

        protected abstract Func<ICorrectionCalculator> CalculatorFactory { get; }

        #endregion
    }
}