using System;
using Core.GasCalculations;
using Devices.Core.Items.ItemGroups;
using Domain.EvcVerifications.CorrectionTests;

namespace Application.ViewModels
{
    public sealed class SuperFactorViewModel : CorrectionTestViewModel<ISuperFactorItems>
    {
        public TemperatureFactorViewModel Temperature { get; set; }
        public PressureFactorViewModel Pressure { get; set; }

        public SuperFactorViewModel(ISuperFactorItems items, TemperatureFactorViewModel temperature, PressureFactorViewModel pressure) : base(items)
        {
            Temperature = temperature;
            Pressure = pressure;

            FactorTestCalculatorDecorator = CorrectionFactory.CreateWithCalculator(CorrectionFactorTestType.Super,
                CalculatorFactory.Invoke(), Items.Factor);
        }

        public override Func<ICorrectionCalculator> CalculatorFactory 
            => () => new SuperFactorCalculator(Items.Co2, Items.N2, Items.SpecGr, Temperature.Gauge, Pressure.Gauge);

        public override void Update(ISuperFactorItems items)
        {
            base.Update(items);
            FactorTest.ActualValue = Items.Factor;
        }
    }
}