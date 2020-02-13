﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using Core.GasCalculations;
using Devices.Core.Items.ItemGroups;
using Domain;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Application.ViewModels
{
    public sealed class TemperatureFactorViewModel : CorrectionTestViewModel<TemperatureItems>
    {
        private const decimal Tolerance = Global.TEMP_ERROR_TOLERANCE;

        [Reactive] public decimal Gauge { get; set; }

        public TemperatureFactorViewModel(TemperatureItems items, decimal gauge) : base(items, Tolerance)
        {
            Gauge = gauge;

            this.WhenAnyValue(x => x.Items)
                .Select(i => i.Factor)
                .ToPropertyEx(this, x => x.ActualValue, Items.Factor);

            this.WhenAnyValue(x => x.Gauge)
                .Select(i => Unit.Default)
                .InvokeCommand(UpdateCommand);
        }

        protected override Func<ICorrectionCalculator> CalculatorFactory
            => () => new TemperatureCalculator(Items.Units, Items.Base, Gauge);

    }
}