namespace Module.EvcVerification.Models.CorrectionTestAggregate
{
    using System;

    /// <summary>
    /// Defines the <see cref="PressureTest"/>
    /// </summary>
    public sealed class PressureTest : BaseCorrectionTest
    {
        #region Fields

        /// <summary>
        /// Defines the _totalGauge
        /// </summary>
        private readonly double? _totalGauge;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PressureTest"/> class.
        /// </summary>
        /// <param name="verificationTest">The verificationTest <see cref="CorrectionTest"/></param>
        /// <param name="percentOfGauge">The percentOfGauge <see cref="double"/></param>
        public PressureTest(IPressureItems items, double percentOfGauge)
        {
            Items = items;

            _totalGauge = GetGaugePressure(percentOfGauge);
            AtmosphericGauge = default(double?);

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

        private PressureTest() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ActualFactor
        /// </summary>
        public override double ActualFactor
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
        public double? AtmosphericGauge { get; set; }

        /// <summary>
        /// Gets the EvcFactor
        /// </summary>
        public override double EvcFactor => Items.Factor;

        /// <summary>
        /// Gets or sets the GasGauge
        /// </summary>
        public double? GasGauge { get; set; }

        /// <summary>
        /// Gets the GasGaugePsi
        /// </summary>
        public double? GasGaugePsi => GasGauge.HasValue ? ConvertToPsi(GasGauge.Value, Items.Units) : default(double?);

        /// <summary>
        /// Gets the GasPressure
        /// </summary>
        public double GasPressure
        {
            get
            {
                var result = GasGauge.GetValueOrDefault(0) + AtmosphericGauge.GetValueOrDefault(0);
                return Math.Round(result, 4);
            }
        }

        public double GasPressurePsi => ConvertToPsi(GasPressure, Items.Units);

        public IPressureItems Items { get; }

        public double TotalGauge => _totalGauge ?? 0;

        /// <summary>
        /// Gets the PassTolerance
        /// </summary>
        protected override double PassTolerance => Global.PRESSURE_ERROR_TOLERANCE;

        #endregion

        #region Methods

        /// <summary>
        /// The ConvertTo
        /// </summary>
        /// <param name="value">The value <see cref="double"/></param>
        /// <param name="fromUnit">The fromUnit <see cref="string"/></param>
        /// <param name="toUnit">The toUnit <see cref="string"/></param>
        /// <returns>The <see cref="double"/></returns>
        public static double ConvertTo(double value, PressureUnits fromUnit, PressureUnits toUnit)
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
        /// <param name="value">The value <see cref="double"/></param>
        /// <param name="fromUnit">The fromUnit <see cref="string"/></param>
        /// <returns>The <see cref="double"/></returns>
        public static double ConvertToPsi(double value, PressureUnits fromUnit)
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
        /// <param name="percentOfGauge">The percentOfGauge <see cref="double"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double GetGaugePressure(double percentOfGauge)
        {
            if (percentOfGauge > 1)
                percentOfGauge = percentOfGauge / 100;

            var evcPressureRange = Items.Range;
            return Math.Round(percentOfGauge * evcPressureRange, 2);
        }

        #endregion
    }
}