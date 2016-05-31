using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prover.Core.Extensions;

namespace Prover.Core.Models.Instruments
{
    public class TemperatureTest : BaseVerificationTest
    {
        private const decimal TempCorrection = 459.67m;
        private const decimal MetericTempCorrection = 273.15m;

        public TemperatureTest() { }

        public TemperatureTest(VerificationTest verificationTest, decimal gauge) : 
            base()
        {
            VerificationTest = verificationTest;
            VerificationTestId = VerificationTest.Id;

            Gauge = (double)gauge;
        }

        public Guid VerificationTestId { get; set; }
        [Required]
        public virtual VerificationTest VerificationTest { get; set; }

        public double Gauge { get; set; }

        public override decimal? PercentError
        {
            get
            {
                if (ItemValues.EvcTemperatureFactor() == null) return null;
                if (ActualFactor == null) return null;
                return Math.Round((decimal) ((ItemValues.EvcTemperatureFactor().GetValueOrDefault(0) - ActualFactor)/ActualFactor)*100, 2);
            }
        }

        public override decimal? ActualFactor
        {
            get
            {

                switch (VerificationTest.Instrument.TemperatureUnits())
                {
                    case "K":
                    case "C":
                        return
                            Math.Round((MetericTempCorrection + VerificationTest.Instrument.EvcBaseTemperature().GetValueOrDefault(0)) / ((decimal)Gauge + MetericTempCorrection), 4);
                    case "R":
                    case "F":
                        return
                            Math.Round((TempCorrection + VerificationTest.Instrument.EvcBaseTemperature().GetValueOrDefault(0)) / ((decimal)Gauge + TempCorrection), 4);
                }

                return 0;
            }
        }
    }
}
