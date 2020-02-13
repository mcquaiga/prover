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
    public sealed class PressureFactorViewModel : CorrectionTestViewModel<PressureItems>
    {
        private const decimal Tolerance = Global.PRESSURE_ERROR_TOLERANCE;

        public PressureFactorViewModel(PressureItems items, decimal gauge, decimal atmosphericGauge) : base(items, Tolerance)
        {
            Gauge = gauge;
            AtmosphericGauge = atmosphericGauge;

            this.WhenAnyValue(x => x.Items)
                .Select(i => i.Factor)
                .ToPropertyEx(this, x => x.ActualValue, Items.Factor);

            this.WhenAnyValue(x => x.Gauge, x => x.AtmosphericGauge)
                .Select(_ => Unit.Default)
                .InvokeCommand(UpdateCommand);
        }

        [Reactive] public decimal Gauge { get; set; }
        [Reactive] public decimal AtmosphericGauge { get; set; }

        protected override Func<ICorrectionCalculator> CalculatorFactory
            => () => new PressureCalculator(Items.UnitType, Items.TransducerType, Items.Base, Gauge, AtmosphericGauge);
        
    }
}