using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Events
{
    public class LiveReadEvent
    {
        public double LiveReadValue { get; set; }

        public LiveReadEvent(double liveReadValue)
        {
            LiveReadValue = liveReadValue;
        }
    }
}
