using Prover.Core.Models.Instruments;

namespace Prover.GUI.Events
{
    public class VerificationTestEvent
    {
        public VerificationTest VerificationTest { get; }

        private VerificationTestEvent(VerificationTest verificationTest)
        {
            VerificationTest = verificationTest;
        }

        public static VerificationTestEvent Raise(VerificationTest verificationTest)
        {
            return new VerificationTestEvent(verificationTest);
        }

        public static VerificationTestEvent Raise()
        {
            return new VerificationTestEvent(null);
        }
    }
}