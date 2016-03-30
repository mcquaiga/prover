using Prover.Core.Communication;
using Prover.Core.VerificationTests;

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