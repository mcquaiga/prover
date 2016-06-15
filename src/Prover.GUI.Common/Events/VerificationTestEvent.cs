namespace Prover.GUI.Common.Events
{
    public class VerificationTestEvent
    {
        private VerificationTestEvent()
        {
        }

        public static VerificationTestEvent Raise()
        {
            return new VerificationTestEvent();
        }
    }
}