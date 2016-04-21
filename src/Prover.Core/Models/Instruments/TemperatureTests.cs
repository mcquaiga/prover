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
    public class TemperatureTest : InstrumentTable
    {
        private const decimal TempCorrection = 459.67m;
        private const decimal MetericTempCorrection = 273.15m;

        public TemperatureTest() { }

        public TemperatureTest(VerificationTest verificationTest) : 
            base()
        {
            VerificationTest = verificationTest;
            VerificationTestId = VerificationTest.Id;
        }

        public Guid VerificationTestId { get; set; }
        [Required]
        public virtual VerificationTest VerificationTest { get; set; }

        public double Gauge { get; set; }
        public decimal? PercentError
        {
            get
            {
                if (ItemValues.EvcTemperatureFactor() == null) return null;
                return Math.Round((decimal) ((ItemValues.EvcTemperatureFactor() - ActualFactor)/ActualFactor)*100, 2);
            }
        }

        public decimal? ActualFactor
        {
            get
            {

                switch (VerificationTest.Instrument.TemperatureUnits())
                {
                    case "K":
                    case "C":
                        return
                            Math.Round(
                                (decimal)
                                    ((MetericTempCorrection + VerificationTest.Instrument.EvcBaseTemperature()) /
                                     ((decimal)Gauge + MetericTempCorrection)), 4);
                    case "R":
                    case "F":
                        return
                            Math.Round(
                                (decimal) ((TempCorrection + VerificationTest.Instrument.EvcBaseTemperature())/((decimal)Gauge + TempCorrection)), 4);
                }

                return 0;
            }
        }
    }
}
