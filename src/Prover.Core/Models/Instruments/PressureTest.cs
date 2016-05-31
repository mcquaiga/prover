using Prover.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Models.Instruments
{ 
    public class PressureTest : BaseVerificationTest
    {
        private const decimal DefaultAtmGauge = 14.0m;

        public PressureTest() { }

        public PressureTest(VerificationTest verificationTest, decimal gauge)
        {
            VerificationTest = verificationTest;
            VerificationTestId = VerificationTest.Id;
            GasGauge = decimal.Round(gauge, 2);
            AtmosphericGauge = decimal.Round(DefaultAtmGauge, 2);
        }
          

        public Guid VerificationTestId { get; set; }
        [Required]
        public virtual VerificationTest VerificationTest { get; set; }

        public decimal? GasPressure
        {
            get
            {
                if (VerificationTest == null) return null;

                var result = 0.0m;
                switch (VerificationTest.Instrument.GetTransducerType())
                {
                    case TransducerType.Gauge:
                        result = GasGauge.GetValueOrDefault(0);
                        break;
                    default:
                        result = GasGauge.GetValueOrDefault(0) + AtmosphericGauge.GetValueOrDefault(0);
                        break;
                }
                return decimal.Round(result, 2);
            } 
        }

        public decimal? GasGauge { get; set; }

        public decimal? AtmosphericGauge { get; set; }

        public override decimal? PercentError
        {
            get
            {
                if (ItemValues.EvcPressureFactor() == null) return null;
                if (ActualFactor == 0 || ActualFactor == null) return null;
                return Math.Round((decimal)((ItemValues.EvcPressureFactor().GetValueOrDefault(0) - ActualFactor) / ActualFactor) * 100, 2);
            }
        }

        [NotMapped]
        public override decimal? ActualFactor
        {
            get
            {
                if (VerificationTest.Instrument.EvcBasePressure() == 0) return 0;
                var result = GasPressure / VerificationTest.Instrument.EvcBasePressure().GetValueOrDefault(1);
                return result.HasValue ? decimal.Round(result.Value, 4) : 0;
            }
        }
    }
}
