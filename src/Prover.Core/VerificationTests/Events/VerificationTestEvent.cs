using Prover.Core.Models.Instruments;
using Prover.Core.Extensions;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.VerificationTests.Events
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
        public VolumeTestStatusEvent(string headerText, VolumeTest volumeTest)
        {
            HeaderText = headerText;
            VolumeTest = volumeTest;
        }

        public string HeaderText { get; }
        public VolumeTest VolumeTest { get; }
    }
}
