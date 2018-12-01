using Prover.Core.VerificationTests;

namespace Prover.GUI.Common.Events
{
    public class InstrumentUpdateEvent
    {
        public InstrumentUpdateEvent(TestRunManager instrumentManager)
        {
            InstrumentManager = instrumentManager;
        }

        public TestRunManager InstrumentManager { get; set; }
    }
}