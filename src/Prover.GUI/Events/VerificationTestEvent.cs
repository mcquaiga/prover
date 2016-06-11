namespace Prover.GUI.Events
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