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
        private const string Therms = "Therms";
        private const string Dktherms = "DecaTherms";
        private const string MegaJoules = " MegaJoules";
        private const string GigaJoules = "GigaJoules";
        private const string KiloCals = "KiloCals";
        private const string KiloWattHours = "KiloWattHours";

        private readonly Instrument _instrument;

        public Energy(Instrument instrument)
        {
            _instrument = instrument;
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
                var startEnergy = _instrument.VolumeTest.Items?.GetItem(140)?.NumericValue;
                var endEnergy = _instrument.VolumeTest.AfterTestItems?.GetItem(140)?.NumericValue;
                if (endEnergy != null && startEnergy != null)
                    return endEnergy.Value - startEnergy.Value;

                return null;
            }
        }

        public string EnergyUnits => _instrument.Items.GetItem(141).Description;

        public decimal? ActualEnergy
        {
            get
            {
                if (!_instrument.VolumeTest.EvcCorrected.HasValue) return null;
                var energyValue = _instrument.Items.GetItem(142).NumericValue;
                switch (EnergyUnits)
                {
                    case Therms:
                        return Math.Round(energyValue * _instrument.VolumeTest.EvcCorrected.Value) / 100000;
                    case Dktherms:
                        return Math.Round(energyValue * _instrument.VolumeTest.EvcCorrected.Value) / 1000000;
                    case GigaJoules:
                        return Math.Round(energyValue * 0.028317m * _instrument.VolumeTest.EvcCorrected.Value) /
                               1000000;
                }

                return null;
            }
        }
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

        public string Discriminator => "Mechanical";

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