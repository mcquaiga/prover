using Devices.Core.Items.ItemGroups;
using Prover.Shared;
using Prover.Shared.Extensions;
using System;

namespace Prover.Calculations
{
    public class PressureCalculator : ICorrectionCalculator
    {
        private readonly decimal _basePressure;

        private readonly PressureTransducerType _transducerType;
        private readonly PressureUnitType _unitType;
        private decimal _atmStandard;

        public PressureCalculator(PressureUnitType unitType, PressureTransducerType transducerType,
            decimal basePressure, decimal atmosphericStandard, decimal gaugePressure, decimal? gaugeAtmospheric)
        {
            _unitType = unitType;
            _transducerType = transducerType;
            _basePressure = basePressure;
            _atmStandard = atmosphericStandard;

            Gauge = gaugePressure;
            GaugeAtmospheric = gaugeAtmospheric ?? 0m;
        }

        public PressureCalculator(PressureItems items, decimal gaugePressure, decimal? gaugeAtmospheric)
        {
            _unitType = items.UnitType;
            _transducerType = items.TransducerType;
            _basePressure = items.Base;
            _atmStandard = items.AtmosphericPressure;

            Gauge = gaugePressure;
            GaugeAtmospheric = gaugeAtmospheric ?? 0m;
        }


        #region Public Properties
        public decimal GaugeAtmospheric { get; set; }

        public decimal Gauge { get; set; }

        public decimal GasPressure => GetGasPressure();

        #endregion

        #region Public Methods

        public decimal CalculateFactor()
        {
            if (_basePressure == 0)
                return 0;

            var psiGas = GasPressure.ConvertToPsi(_unitType);
            var psiBase = _basePressure.ConvertToPsi(_unitType);

            FactorFormula = $"{psiGas} (Gas P) / {psiBase} (Base)";

            return Round.Factor(psiGas / psiBase);
        }

        public string FactorFormula { get; private set; }
        public string GasPressureFormula { get; private set; }
        /// <summary>
        ///     Gets the GasPressure
        /// </summary>
        /// <param name="transducerType"></param>
        /// <param name="gauge"></param>
        /// <param name="atmosphericGauge"></param>
        /// <param name="items"></param>
        public decimal GetGasPressure()
        {
            switch (_transducerType)
            {
                case PressureTransducerType.Gauge:
                    GasPressureFormula = $"(Gauge) {Gauge} + (Atm Standard) {_atmStandard}";
                    return Round.Gauge(Gauge + _atmStandard);

                case PressureTransducerType.Absolute:
                    GasPressureFormula = $"(Gauge) {Gauge} + (Atm Gauge) {GaugeAtmospheric}";
                    return Round.Gauge(Gauge);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static decimal GetGaugePressure(int range, decimal percentOfRange)
        {
            if (percentOfRange > 1)
                percentOfRange /= 100;

            return Round.Gauge(percentOfRange * range);
        }

        #endregion
    }

    public static class PressureUnitExtensions
    {
        #region Public Methods

        public static decimal ConvertToPsi(this decimal value, PressureUnitType fromUnit)
        {
            decimal result;
            switch (fromUnit)
            {
                case PressureUnitType.BAR:
                    result = value / (6.894757m * (10 ^ -2));
                    break;

                case PressureUnitType.inWC:
                    result = value / 27.68067m;
                    break;

                case PressureUnitType.KGcm2:
                    result = value / (7.030696m * (10 ^ -2));
                    break;

                case PressureUnitType.kPa:
                    result = value / 6.894757m;
                    break;

                case PressureUnitType.mBAR:
                    result = value / (6.894757m * (10 ^ 1));
                    break;

                case PressureUnitType.mPa:
                    result = value / (6.894757m * (10 ^ -3));
                    break;

                case PressureUnitType.inHG:
                    result = value / 2.03602m;
                    break;

                case PressureUnitType.mmHG:
                    result = value / 51.71492m;
                    break;

                default:
                    result = value / 1;
                    break;
            }

            return Round.Gauge(result);
        }

        public static decimal ConvertToUnits(decimal value, PressureUnitType from, PressureUnitType to)
        {
            var result = ConvertToPsi(value, from);

            switch (to)
            {
                case PressureUnitType.kPa:
                    result *= 6.894757m;
                    break;

                case PressureUnitType.mPa:
                    result *= 6.894757m * (10 ^ -3);
                    break;

                case PressureUnitType.BAR:
                    result *= 6.894757m * (10 ^ -2);
                    break;

                case PressureUnitType.mBAR:
                    result *= 6.894757m * (10 ^ 1);
                    break;

                case PressureUnitType.KGcm2:
                    result *= 7.030696m * (10 ^ -2);
                    break;

                case PressureUnitType.inWC:
                    result *= 27.68067m;
                    break;

                case PressureUnitType.inHG:
                    result *= 2.03602m;
                    break;

                case PressureUnitType.mmHG:
                    result *= 51.71492m;
                    break;
            }

            return Round.Gauge(result);
        }

        #endregion
    }
}