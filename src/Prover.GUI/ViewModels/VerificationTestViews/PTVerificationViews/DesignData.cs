using Prover.Core.Models.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.GUI.ViewModels.VerificationTestViews.PTVerificationViews
{
    public class TemperatureTestDesignData
    {
        public TemperatureTest TempTest { get; private set; }

        public TemperatureTestDesignData()
        {
            this.Test = new TemperatureTest
            {
                Gauge = 90
            };
        }

        public double Gauge => 90.0;
       

        public decimal? EvcReading => 90m;
        public decimal? EvcFactor => 1.000m;

        public TemperatureTest Test { get; private set; }
    }
}
