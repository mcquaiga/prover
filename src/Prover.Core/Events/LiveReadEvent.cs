using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Events
{
    public class LiveReadEvent
    {
        public decimal LiveReadValue { get; set; }
        public int ItemNumber { get; set; }

        public LiveReadEvent(int itemNumber, decimal liveReadValue)
        {
            LiveReadValue = liveReadValue;
            ItemNumber = itemNumber;
        }
    }
}
