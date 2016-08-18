using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.Collections;

namespace Prover.Core.VerificationTests
{
    public class ReadingStabilizer
    {
        private const decimal AverageThreshold = 1.0m;
        private const int FixedQueueSize = 20;

        public ReadingStabilizer(decimal gaugeValue)
        {
            this.GaugeValue = gaugeValue;
            this.ValueQueue = new FixedSizedQueue<decimal>(FixedQueueSize);
        }

        public FixedSizedQueue<decimal> ValueQueue { get; set; }

        public decimal GaugeValue { get; private set; }

        public bool IsStable
        {
            get
            {
                if (ValueQueue.Count == FixedQueueSize)
                {
                    var average = ValueQueue.Sum() / FixedQueueSize;
                    var difference = Math.Abs(GaugeValue - average);

                    if (difference <= AverageThreshold)
                    {
                        return true;
                    }
                }

                return false;
            }           
        }
    }
}
