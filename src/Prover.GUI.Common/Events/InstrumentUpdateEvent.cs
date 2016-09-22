using Prover.Core.VerificationTests.VolumeTest;

namespace Prover.GUI.Common.Events
{
    public class InstrumentUpdateEvent
    {
        public InstrumentUpdateEvent(RotaryTestManager instrumentManager)
        {
            InstrumentManager = instrumentManager;
        }

        public RotaryTestManager InstrumentManager { get; set; }
    }
}