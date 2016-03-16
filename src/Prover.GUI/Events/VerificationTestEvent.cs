using Prover.Core.Communication;

namespace Prover.GUI.Events
{
    public class VerificationTestEvent
    {
        public VerificationTestEvent(TestManager testManager)
        {
            this.TestManager = testManager;
        }

        public TestManager TestManager { get; private set; }
    }
}