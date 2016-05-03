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
        private decimal? _atmGauge;
      
        public PressureTest() { }

        public PressureTest(VerificationTest verificationTest)
        {
            VerificationTest = verificationTest;
            VerificationTestId = VerificationTest.Id;
            GasGauge = 0;
            AtmosphericGauge = 0;
        }
          

        public Guid VerificationTestId { get; set; }
        [Required]
        public virtual VerificationTest VerificationTest { get; set; }

        public decimal? GasPressure
        {
            get
            {
                if (VerificationTest == null) return null;

                switch (VerificationTest.Instrument.GetTransducerType())
                {
                    case TransducerType.Gauge:
                        return GasGauge;
                    default:
                        return GasGauge + AtmosphericGauge;
                }
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
                return Math.Round((decimal)((ItemValues.EvcPressureFactor() - ActualFactor) / ActualFactor) * 100, 2);
            }
        }

        [NotMapped]
        public override decimal? ActualFactor
        {
            get
            {
                if (VerificationTest.Instrument.EvcBasePressure() == 0) return 0;
                var result = GasPressure / VerificationTest.Instrument.EvcBasePressure();
                return result.HasValue ? decimal.Round(result.Value, 4) : result;
            }
        }

       
    }
}
