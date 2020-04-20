using Domain.EvcVerifications;

namespace Module.EvcVerification.VerificationTests.Events
{
    public class VerificationTestEvent
    {
        public string Message { get; }

        public VerificationTestEvent(string message)
        {
            Message = message;
        }
    }

    public class VolumeTestStatusEvent
    {
        public VolumeTestStatusEvent(string headerText, VerificationTestPoint testPoint)
        {
            HeaderText = headerText;
            TestPoint = testPoint;
        }

        public string HeaderText { get; }
        public VerificationTestPoint TestPoint { get; }
    }
}
