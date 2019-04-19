using System;
using System.Linq;
using Prover.Core.Models.Instruments;
using Prover.Core.Models.Instruments.DriveTypes;

namespace Prover.Core.DriveTypes
{
    public class PulseInputSensor : IDriveType
    {
        public PulseInputSensor(Instrument instrument)
        {
            Instrument = instrument; 
            Energy = new Energy(Instrument);
        }

        public string Discriminator => Drives.PulseInput;

        public bool HasPassed => true;

        public FrequencyTest FrequencyTest => Instrument.VerificationTests.FirstOrDefault(x => x.FrequencyTest != null)?.FrequencyTest;
        public Instrument Instrument { get; }
        public Energy Energy { get; }

        Models.Instruments.DriveTypes.Energy IDriveType.Energy => throw new NotImplementedException();

        public int MaxUncorrectedPulses()
        {
            return 10;
        }

        public decimal? UnCorrectedInputVolume(decimal appliedInput)
        {
            return FrequencyTest.AdjustedVolume();
        }
    }
}
