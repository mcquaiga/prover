using System;
using System.Reactive;
using System.Reactive.Linq;
using Core.GasCalculations;
using Devices.Core.Items.ItemGroups;
using Domain;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Application.ViewModels
{
    public sealed class SuperFactorViewModel : CorrectionTestViewModel<SuperFactorItems>
    {
        private const decimal Tolerance = Global.SUPER_FACTOR_TOLERANCE;

        public SuperFactorViewModel(SuperFactorItems items, TemperatureFactorViewModel temperature, PressureFactorViewModel pressure)
            : base(items, Tolerance)
        {
            Temperature = temperature;
            Pressure = pressure;

            this.WhenAnyValue(x => x.Pressure.Items)
                .Select(i => i.UnsqrFactor)
                .ToPropertyEx(this, x => x.ActualValue, Pressure.Items.UnsqrFactor);

            this.WhenAnyValue(x => x.Temperature.Gauge, x => x.Pressure.Gauge, x => x.Pressure.AtmosphericGauge)
                .Select(_ => Unit.Default)
                .InvokeCommand(UpdateCommand);

            this.WhenAnyValue(x => x.ExpectedValue)
                .Select(Calculators.SquaredFactor)
                .ToPropertyEx(this, x => x.SquaredFactor, deferSubscription: true);
        }

        #region Public Properties

        public extern decimal SquaredFactor { [ObservableAsProperty] get; }

        public TemperatureFactorViewModel Temperature { get; }
        public PressureFactorViewModel Pressure { get; }

        #endregion

        #region Protected

        protected override Func<ICorrectionCalculator> CalculatorFactory
            => () => new SuperFactorCalculator(Items.Co2, Items.N2, Items.SpecGr, Temperature.Gauge, Pressure.Gauge);

        #endregion
    }
}