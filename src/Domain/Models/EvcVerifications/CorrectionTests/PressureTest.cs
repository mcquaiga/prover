using System;
using Devices.Core;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items.ItemGroups;
using Domain.Calculators.Helpers;

namespace Domain.Models.EvcVerifications.CorrectionTests
{
    /// <summary>
    /// Defines the <see cref="PressureTest"/>
    /// </summary>
    public class PressureTest : CorrectionBase<IPressureItems>
    {
        public PressureTest(IPressureItems values, decimal actual, decimal expected, decimal atmosphericGauge, decimal gasGauge) : base(values, actual, expected)
        {
            AtmosphericGauge = atmosphericGauge;
            GasGauge = gasGauge;
        }

        /// <summary>
        /// Gets or sets the AtmosphericGauge
        /// </summary>
        public decimal AtmosphericGauge { get; set; }

        /// <summary>
        /// Gets or sets the GasGauge
        /// </summary>
        public decimal GasGauge { get; set; }



        /// <summary>
        /// Gets the PassTolerance
        /// </summary>
        public override decimal PassTolerance => Global.PRESSURE_ERROR_TOLERANCE;

        public decimal TotalGauge => _totalGauge ?? 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="PressureTest"/> class.
        /// </summary>
        /// <param name="verificationTest">The verificationTest <see cref="CorrectionTestPoint"/></param>
        /// <param name="percentOfGauge">The percentOfGauge <see cref="decimal"/></param>
        //public PressureTest(IPressureItems items, decimal percentOfGauge)
        //{
        //    Items = items;

        //    _totalGauge = GetGaugePressure(percentOfGauge);
        //    AtmosphericGauge = default(decimal?);

        //    switch (Items.TransducerType)
        //    {
        //        case PressureTransducerType.Gauge:
        //            GasGauge = TotalGauge;
        //            AtmosphericGauge = Items.AtmosphericPressure;
        //            break;

        //        case PressureTransducerType.Absolute:
        //            AtmosphericGauge = null;
        //            GasGauge = TotalGauge - (AtmosphericGauge ?? 0);
        //            break;
        //    }
        //}
    }
}