using System;
using System.Reactive;
using System.Reactive.Linq;
using Core.GasCalculations;
using Devices.Core.Items.ItemGroups;
using Domain;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Application.ViewModels.Corrections
{
    public sealed class SuperFactorViewModel : CorrectionTestViewModel<SuperFactorItems>
    {
        private const decimal Tolerance = Global.SUPER_FACTOR_TOLERANCE;


        public SuperFactorViewModel(SuperFactorItems items) : base(items, Tolerance)
        {
            this.WhenAnyValue(x => x.ExpectedValue)
                .Select(Calculators.SquaredFactor)
                .ToPropertyEx(this, x => x.SquaredFactor);
        }

        public SuperFactorViewModel(SuperFactorItems items, TemperatureFactorViewModel temperature, PressureFactorViewModel pressure)
            : this(items)
        {
            Temperature = temperature;
            Pressure = pressure;

            Setup(Temperature, Pressure);
        }

        public extern decimal SquaredFactor { [ObservableAsProperty] get; }

        [Reactive] public TemperatureFactorViewModel Temperature { get; protected set; }
        [Reactive] public PressureFactorViewModel Pressure { get; protected set; }

        protected override Func<ICorrectionCalculator> CalculatorFactory
            => () => new SuperFactorCalculator(Items.Co2, Items.N2, Items.SpecGr, Temperature.Gauge, Pressure.Gauge);

        public void Setup(TemperatureFactorViewModel tempViewModel, PressureFactorViewModel pressureViewModel)
        {
            Temperature = tempViewModel;
            Pressure = pressureViewModel;

            this.WhenAnyValue(x => x.Pressure.Items)
                .Where(i => i != null)
                .Select(i => i.UnsqrFactor)
                .ToPropertyEx(this, x => x.ActualValue, Pressure.Items.UnsqrFactor);
            
            this.WhenAnyValue(x => x.Temperature.Gauge, x => x.Pressure.Gauge, x => x.Pressure.AtmosphericGauge)
                .Select(_ => Unit.Default)
                .InvokeCommand(UpdateFactor);
        }
    }
}