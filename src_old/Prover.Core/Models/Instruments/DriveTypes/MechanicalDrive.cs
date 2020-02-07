using System;
using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Extensions;
using Prover.Core.Settings;

namespace Prover.Core.Models.Instruments.DriveTypes
{
    public class Energy
    {
        public enum Units
        {
            Therms,
            Dktherms,
            MegaJoules,
            GigaJoules,
            KiloCals,
            KiloWattHours
        }

        private readonly Instrument _instrument;
        private decimal? _evcCorrected;

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

        public Units EnergyUnits { get; }

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
                    if (!_instrument.VolumeTest.EvcCorrected.HasValue)
                        return null;

                    _evcCorrected = _instrument.VolumeTest?.EvcCorrected.Value;
                    TotalValue = _instrument.Items.GetItem(142).NumericValue;
                }          

                switch (EnergyUnits)
                {
                    case Units.Therms:
                        return Math.Round(TotalValue * _evcCorrected.Value / 100000);

                    case Units.Dktherms:
                        return Math.Round(TotalValue * _evcCorrected.Value / 1000000) ;

                    case Units.GigaJoules:
                        return Math.Round(TotalValue * 0.028317m * _evcCorrected.Value / 1000000);

                    case Units.MegaJoules:
                        return Math.Round(TotalValue * 0.028317m * _evcCorrected.Value / 1000);

                    case Units.KiloCals:
                        return Math.Round(TotalValue * 0.0283168m * _evcCorrected.Value);

                    default:
                        throw new Exception(string.Format("Energy units not supported: {0}", EnergyUnits));
                }
            }
        }

        public decimal? StartValue { get; private set; }
        public decimal? EndValue { get; private set; }
        public decimal TotalValue { get; private set; }
      
    }

    public class MechanicalDrive : IDriveType
    {
        private readonly List<TestSettings.MechanicalUncorrectedTestLimit> _mechanicalUncorrectedTestLimits;

        public MechanicalDrive(Instrument instrument)
        {
            Instrument = instrument;
            Energy = new Energy(instrument);
        }

        public MechanicalDrive(Instrument instrument, List<TestSettings.MechanicalUncorrectedTestLimit> mechanicalUncorrectedTestLimits)
            : this(instrument)
        {
            _mechanicalUncorrectedTestLimits = mechanicalUncorrectedTestLimits;         
        }

        public Energy Energy { get; set; }

        public Instrument Instrument { get; }

        public string Discriminator => Drives.Mechanical;

        public bool HasPassed => Energy.HasPassed;

        public int MaxUncorrectedPulses()
        {
            var uncorUnitValue = (int) Instrument.Items.GetItem(98).NumericValue;

            return _mechanicalUncorrectedTestLimits?
                       .FirstOrDefault(x => x.CuFtValue == uncorUnitValue)?.UncorrectedPulses ?? 10;
        }

        public decimal? UnCorrectedInputVolume(decimal appliedInput)
        {
            return appliedInput * Instrument.DriveRate();
        }
    }
}