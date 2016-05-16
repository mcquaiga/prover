using Prover.Core.Communication;
using Prover.Core.VerificationTests;

namespace Prover.GUI.Events
{
    public class VerificationTestEvent
    {
        public static VerificationTestEvent Raise()
        {
            return new VerificationTestEvent();
        }
        private VerificationTestEvent() { }
    }
}