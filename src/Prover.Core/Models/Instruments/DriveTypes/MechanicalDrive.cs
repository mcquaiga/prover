using System;
using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Extensions;
using Prover.Core.Settings;

namespace Prover.Core.Models.Instruments.DriveTypes
{
    public class MechanicalDrive : IDriveType
    {
        public string Discriminator => Drives.Mechanical;
        public Energy Energy { get; set; }
        public Instrument Instrument { get; }
        public bool HasPassed => Energy.HasPassed;

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

        public int MaxUncorrectedPulses()
        {
            var uncorUnitValue = (int)Instrument.Items.GetItem(98).NumericValue;

            return _mechanicalUncorrectedTestLimits?
                       .FirstOrDefault(x => x.CuFtValue == uncorUnitValue)?.UncorrectedPulses ?? 10;
        }

        public decimal? UnCorrectedInputVolume(decimal appliedInput)
        {
            return appliedInput * Instrument.DriveRate();
        }

        private readonly List<TestSettings.MechanicalUncorrectedTestLimit> _mechanicalUncorrectedTestLimits;
    }
}