using Devices.Core.Items.DriveTypes;
using Prover.Calculations;

namespace Prover.Application.Models.EvcVerifications.Verifications.Volume
{
    public sealed class EnergyTest : VerificationTestEntity
    {
        //public EnergyTest(IEnergyItems startValues, IEnergyItems endValues, decimal evcCorrected, decimal energyGasValue) 
        //    : base(VerificationTestType.Energy, startValues, endValues)
        //{
        //    ActualValue = endValues.EnergyReading - startValues.EnergyReading;
        //    ExpectedValue = EnergyCalculator.Calculated(startValues.EnergyUnitType, evcCorrected, energyGasValue);
        //}

        #region Public Methods

        //public EnergyTest(EnergyItems startValues, EnergyItems endValues, decimal expectedValue, decimal actualValue, decimal percentError, bool verified) 
        //    : base(startValues, endValues, expectedValue, actualValue, percentError, verified)
        //{
        //}

        public EnergyTest(EnergyItems startValues, EnergyItems endValues, decimal totalCorrectedVolume)
        {
            ActualValue = EnergyCalculator.TotalEnergy(startValues.EnergyReading, endValues.EnergyReading);

            ExpectedValue = EnergyCalculator.Calculated(startValues.EnergyUnitType,
            totalCorrectedVolume,
            startValues.EnergyGasValue);

            Update(Tolerances.ENERGY_PASS_TOLERANCE);
        }

        public EnergyTest(){}
        //public static EnergyTest Create(IEnergyItems startValues, IEnergyItems endValues, decimal? evcCorrected)
        //{
        //    return new EnergyTest(startValues, endValues, evcCorrected ?? 0, endValues.EnergyGasValue);
        //}

        #endregion
    }
}

/*
 *
 *    public class Energy
    {
        public enum Units
        {
            Therms,
            DecaTherms,
            MegaJoules,
            GigaJoules,
            KiloCals,
            KiloWattHours
        }

        public decimal? ActualEnergy
        {
            get
            {
                if (!_evcCorrected.HasValue && _instrument != null && !_instrument.VolumeTest.EvcCorrected.HasValue
                    && !_instrument.VolumeTest.EvcCorrected.HasValue && _instrument.VolumeTest.EvcCorrected == 0)
                {
                    return 0.0m;
                }

                if (_instrument != null)
                {
                    _evcCorrected = _instrument.VolumeTest.EvcCorrected;
                    TotalValue = _instrument.Items.GetItem(142).NumericValue;
                }

                var evcCorrected = _evcCorrected.HasValue ? _evcCorrected.Value : 0;

                switch (EnergyUnits)
                {
                    case Units.Therms:
                        return Math.Round(TotalValue * evcCorrected / 100000);

                    case Units.DecaTherms:
                        return Math.Round(TotalValue * evcCorrected / 1000000);

                    case Units.GigaJoules:
                        return Math.Round(TotalValue * 0.028317m * evcCorrected / 1000000);

                    case Units.MegaJoules:
                        return Math.Round(TotalValue * 0.028317m * evcCorrected / 1000);

                    case Units.KiloCals:
                        return Math.Round(TotalValue * 0.0283168m * evcCorrected);

                    default:
                        throw new Exception(string.Format("Energy units not supported: {0}", EnergyUnits));
                }
            }
        }

        public decimal? EndValue { get; private set; }
        public Units EnergyUnits { get; }

        public decimal? EvcEnergy
        {
            get
            {
                if (_instrument != null)
                {
                    StartValue = _instrument.VolumeTest.Items?.GetItem(140)?.NumericValue;
                    EndValue = _instrument.VolumeTest.AfterTestItems?.GetItem(140)?.NumericValue;
                }

                if (StartValue.HasValue && EndValue.HasValue)
                {
                    return EndValue.Value - StartValue.Value;
                }

                return null;
            }
        }

        public bool HasPassed => PercentError < 1 && PercentError > -1;

        public decimal? PercentError
        {
            get
            {
                if (ActualEnergy == 0) return null;
                var error = (EvcEnergy - ActualEnergy) / ActualEnergy * 100;
                if (error != null)
                    return decimal.Round(error.Value, 2);
                return null;
            }
        }

        public decimal? StartValue { get; private set; }
        public decimal TotalValue { get; private set; }

        public Energy(Instrument instrument)
        {
            _instrument = instrument;
            EnergyUnits = (Units)Enum.Parse(typeof(Units), _instrument.Items.GetItem(141).Description);
        }

        public Energy(Units unitValue, decimal startValue, decimal endValue, decimal totalValue, decimal evcCorrected)
        {
            StartValue = startValue;
            EndValue = endValue;
            TotalValue = totalValue;
            EnergyUnits = unitValue;
            _evcCorrected = evcCorrected;
        }

        private readonly Instrument _instrument;
        private decimal? _evcCorrected;
    }
 *
 *
 */