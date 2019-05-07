namespace Module.EvcVerification.Models.CorrectionTests
{
    using Devices.Core;
    using Devices.Core.Interfaces.Items;
    using System;

    /// <summary>
    /// Defines the <see cref="PressureTest"/>
    /// </summary>
    public sealed class PressureTest : CorrectionBase
    {
        /// <summary>
        /// Gets the ActualFactor
        /// </summary>
        public override decimal Actual
        {
            get
            {
                var basePressure = Items.Base;
                if (basePressure == 0) return 0;

                return Math.Round(GasPressure / basePressure, 4);
            }
        }

        /// <summary>
        /// Gets or sets the AtmosphericGauge
        /// </summary>
        public decimal? AtmosphericGauge { get; set; }

        /// <summary>
        /// Gets the EvcFactor
        /// </summary>
        public override decimal EvcFactor => Items.Factor;

        /// <summary>
        /// Gets or sets the GasGauge
        /// </summary>
        public decimal? GasGauge { get; set; }

        /// <summary>
        /// Gets the GasGaugePsi
        /// </summary>
        public decimal? GasGaugePsi => GasGauge.HasValue ? ConvertToPsi(GasGauge.Value, Items.Units) : default(decimal?);

        /// <summary>
        /// Gets the GasPressure
        /// </summary>
        public decimal GasPressure
        {
            get
            {
                var result = GasGauge.GetValueOrDefault(0) + AtmosphericGauge.GetValueOrDefault(0);
                return Math.Round(result, 4);
            }
        }

        public decimal GasPressurePsi => ConvertToPsi(GasPressure, Items.Units);

        public IPressureItems Items { get; }

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
        public PressureTest(IPressureItems items, decimal percentOfGauge)
        {
            Items = items;

            _totalGauge = GetGaugePressure(percentOfGauge);
            AtmosphericGauge = default(decimal?);

            switch (Items.TransducerType)
            {
                case PressureTransducerType.Gauge:
                    GasGauge = TotalGauge;
                    AtmosphericGauge = Items.AtmosphericPressure;
                    break;

                case PressureTransducerType.Absolute:
                    AtmosphericGauge = null;
                    GasGauge = TotalGauge - (AtmosphericGauge ?? 0);
                    break;
            }
        }

        /// <summary>
        /// The ConvertTo
        /// </summary>
        /// <param name="value">The value <see cref="decimal"/></param>
        /// <param name="fromUnit">The fromUnit <see cref="string"/></param>
        /// <param name="toUnit">The toUnit <see cref="string"/></param>
        /// <returns>The <see cref="decimal"/></returns>
        public static decimal ConvertTo(decimal value, PressureUnits fromUnit, PressureUnits toUnit)
        {
            var result = ConvertToPsi(value, fromUnit);

            switch (toUnit)
            {
                case PressureUnits.BAR:
                    result = result * (6.894757 * (10 ^ -2));
                    break;

                case PressureUnits.inWC:
                    result = result * 27.68067;
                    break;

                case PressureUnits.KGcm2:
                    result = result * (7.030696 * (10 ^ -2));
                    break;

                case PressureUnits.kPa:
                    result = result * 6.894757;
                    break;

                case PressureUnits.mBAR:
                    result = result * (6.894757 * (10 ^ 1));
                    break;

                case PressureUnits.mPa:
                    result = result * (6.894757 * (10 ^ -3));
                    break;

                case PressureUnits.inHG:
                    result = result * 2.03602;
                    break;

                case PressureUnits.mmHG:
                    result = result * 51.71492;
                    break;

                default:
                    result = result / 1;
                    break;
            }

            return Math.Round(result, 2);
        }

        /// <summary>
        /// The ConvertToPsi
        /// </summary>
        /// <param name="value">The value <see cref="decimal"/></param>
        /// <param name="fromUnit">The fromUnit <see cref="string"/></param>
        /// <returns>The <see cref="decimal"/></returns>
        public static decimal ConvertToPsi(decimal value, PressureUnits fromUnit)
        {
            var result = 0.0;

            switch (fromUnit)
            {
                case PressureUnits.BAR:
                    result = value / (6.894757 * (10 ^ -2));
                    break;

                case PressureUnits.inWC:
                    result = value / 27.68067;
                    break;

                case PressureUnits.KGcm2:
                    result = value / (7.030696 * (10 ^ -2));
                    break;

                case PressureUnits.kPa:
                    result = value / 6.894757;
                    break;

                case PressureUnits.mBAR:
                    result = value / (6.894757 * (10 ^ 1));
                    break;

                case PressureUnits.mPa:
                    result = value / (6.894757 * (10 ^ -3));
                    break;

                case PressureUnits.inHG:
                    result = value / 2.03602;
                    break;

                case PressureUnits.mmHG:
                    result = value / 51.71492;
                    break;

                default:
                    result = value / 1;
                    break;
            }

            return Math.Round(result, 2);
        }

        /// <summary>
        /// The GetGaugePressure
        /// </summary>
        /// <param name="percentOfGauge">The percentOfGauge <see cref="decimal"/></param>
        /// <returns>The <see cref="decimal"/></returns>
        public decimal GetGaugePressure(decimal percentOfGauge)
        {
            if (percentOfGauge > 1)
                percentOfGauge = percentOfGauge / 100;

            var evcPressureRange = Items.Range;
            return Math.Round(percentOfGauge * evcPressureRange, 2);
        }

        /// <summary>
        /// Defines the _totalGauge
        /// </summary>
        private readonly decimal? _totalGauge;

        private PressureTest()
        {
        }
    }
}