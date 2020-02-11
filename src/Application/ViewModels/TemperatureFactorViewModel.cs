﻿using System;
using Core.GasCalculations;
using Devices.Core.Items.ItemGroups;
using Domain.EvcVerifications.CorrectionTests;

namespace Application.ViewModels
{
    public sealed class TemperatureFactorViewModel : CorrectionTestViewModel<ITemperatureItems>
    {
        public decimal Gauge { get; set; }

        public TemperatureFactorViewModel(ITemperatureItems items, decimal gauge) : base(items)
        {
            Gauge = gauge;
            FactorTestCalculatorDecorator = CorrectionTest.CreateWithCalculator(CorrectionFactorTestType.Temperature, CalculatorFactory.Invoke(),
                Items.Factor, Gauge);
        }

        public override Func<ICorrectionCalculator> CalculatorFactory 
            => () => new TemperatureCalculator(Items.Units, Items.Base, Gauge);

        public override void Update(ITemperatureItems items)
        {
            base.Update(items);
            ((CorrectionTestWithGauge) FactorTest).Gauge = Gauge;
            FactorTest.ActualFactor = Items.Factor;
        }
    }
}