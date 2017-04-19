namespace Prover.Client.Framework.Events
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