using System;
using Core.GasCalculations;
using Devices.Core.Items.ItemGroups;
using Domain.EvcVerifications.CorrectionTests;

namespace Application.ViewModels
{
    public sealed class PressureFactorViewModel : CorrectionTestViewModel<IPressureItems>
    {
        public PressureFactorViewModel(IPressureItems items, decimal gauge, decimal atmosphericGauge) : base(items)
        {
            Gauge = gauge;
            AtmosphericGauge = atmosphericGauge;

            FactorTestCalculatorDecorator = CorrectionTest.CreateWithCalculator(CorrectionFactorTestType.Pressure, CalculatorFactory.Invoke(), Items.Factor, Gauge);
        }

        public decimal Gauge { get; set; }
        public decimal AtmosphericGauge { get; set; }
        public override Func<ICorrectionCalculator> CalculatorFactory
            => () => new PressureCalculator(Items.UnitType, Items.TransducerType, Items.Base, Gauge, AtmosphericGauge);

        public override void Update(IPressureItems items)
        {
            base.Update(items);
            ((CorrectionTestWithGauge) FactorTest).Gauge = Gauge;
            FactorTest.ActualFactor = Items.Factor;
        }
    }
}