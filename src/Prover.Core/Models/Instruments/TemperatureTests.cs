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
        private Instrument _instrument;

        public TemperatureTest(VerificationTest verificationTest) : 
            base(verificationTest.Instrument.Items.CopyItemsByFilter(x => x.IsTemperatureTest == true))
        {
            VerificationTest = verificationTest;
            _instrument = VerificationTest.Instrument;
        }

        [NotMapped]
        public VerificationTest VerificationTest { get; set; }

        public double Gauge { get; set; }
        public decimal? PercentError
        {
            get
            {
                if (Items.EvcTemperatureFactor() == null) return null;
                return Math.Round((decimal) ((Items.EvcTemperatureFactor() - ActualFactor)/ActualFactor)*100, 2);
            }
        }

        public decimal? ActualFactor
        {
            get
            {
                switch (_instrument.TemperatureUnits())
                {
                    case "K":
                    case "C":
                        return
                            Math.Round(
                                (decimal)
                                    ((MetericTempCorrection + _instrument.EvcBaseTemperature()) /
                                     ((decimal)Gauge + MetericTempCorrection)), 4);
                    case "R":
                    case "F":
                        return
                            Math.Round(
                                (decimal) ((TempCorrection + _instrument.EvcBaseTemperature())/((decimal)Gauge + TempCorrection)), 4);
                }

                return 0;
            }
        }
    }
}
