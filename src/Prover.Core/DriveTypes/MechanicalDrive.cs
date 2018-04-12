using System.Linq;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;

namespace Prover.Core.DriveTypes
{
    public class MechanicalDrive : IDriveType
    {
        public MechanicalDrive(Instrument instrument)
        {
            Instrument = instrument;
            Energy = new Energy(instrument);
        }

        public Energy Energy { get; set; }

        public Instrument Instrument { get; }

        public string Discriminator => DriveTypes.Mechanical;

        public bool HasPassed => Energy.HasPassed;

        public int MaxUncorrectedPulses()
        {
            var uncorPulseTable = SettingsManager.SettingsInstance.MechanicalUncorrectedTestLimits;
            var uncorUnitValue = (int) Instrument.Items.GetItem(98).NumericValue;

            return uncorPulseTable.FirstOrDefault(x => x.CuFtValue == uncorUnitValue)?.UncorrectedPulses ?? 10;
        }

        public decimal? UnCorrectedInputVolume(decimal appliedInput)
        {
            return appliedInput * Instrument.DriveRate();
        }
    }
}