using Prover.Core.VerificationTests.VolumeTest;

namespace Prover.GUI.Common.Events
{
    public class InstrumentUpdateEvent
    {
        public InstrumentUpdateEvent(RotaryQaRunTestManager instrumentManager)
        {
            InstrumentManager = instrumentManager;
        }

        public RotaryQaRunTestManager InstrumentManager { get; set; }
    }
}