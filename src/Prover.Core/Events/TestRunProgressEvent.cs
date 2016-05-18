using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Events
{
    public class TestRunProgressEvent
    {
        public enum TestRunStep
        {
            InstrumentCommunication,
            LiveReading,
            RunningVolumeTest
        }

        public TestRunProgressEvent(TestRunStep step)
        {
            this.TestStep = step;
        }

        public TestRunStep TestStep { get; private set; }
    }
}
