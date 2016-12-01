using Prover.Core.VerificationTests;

namespace Prover.GUI.Common.Events
{
    public class InstrumentUpdateEvent
    {
        public InstrumentUpdateEvent(QaRunTestManager instrumentManager)
        {
            InstrumentManager = instrumentManager;
        }

        public QaRunTestManager InstrumentManager { get; set; }
    }
}