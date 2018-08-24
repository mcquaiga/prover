using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;

namespace Prover.Core.DriveTypes
{
    public class PulseInputSensor : IDriveType
    {
        public PulseInputSensor(Instrument instrument)
        {
            Instrument = instrument; 
            Energy = new Energy(Instrument);
        }

        public string Discriminator => DriveTypes.PulseInput;

        public bool HasPassed => true;

        public FrequencyTest FrequencyTest => Instrument.VerificationTests.FirstOrDefault(x => x.FrequencyTest != null)?.FrequencyTest;
        public Instrument Instrument { get; }
        public Energy Energy { get; }

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
