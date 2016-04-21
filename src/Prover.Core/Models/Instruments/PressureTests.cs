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
    public class PressureTest : InstrumentTable
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

        public decimal? GasGauge { get; set; }
        public decimal? AtmosphericGauge
        {
            get
            {
                if (VerificationTest == null) return null;
                switch (VerificationTest.Instrument.GetTransducerType())
                {
                    case TransducerType.Gauge:
                        return VerificationTest.Instrument.EvcAtmosphericPressure();
                    case TransducerType.Absolute:
                        return _atmGauge;
                    default:
                        return VerificationTest.Instrument.EvcAtmosphericPressure();
                }
            }
            set
            {
                _atmGauge = value;
            }
        }
        public decimal? PercentError
        {
            get
            {
                if (ItemValues.EvcPressureFactor() == null) return null;
                if (ActualFactor == 0 || ActualFactor == null) return null;
                return Math.Round((decimal)((ItemValues.EvcPressureFactor() - ActualFactor) / ActualFactor) * 100, 2);
            }
        }

        [NotMapped]
        public decimal? ActualFactor
        {
            get
            {
                if (VerificationTest.Instrument.EvcBasePressure() == 0) return 0;
                var result = (GasGauge + AtmosphericGauge) / VerificationTest.Instrument.EvcBasePressure();
                return result.HasValue ? decimal.Round(result.Value, 4) : result;
            }
        }       
        [NotMapped]
        public bool HasPassed
        {
            get { return (PercentError < 1 && PercentError > -1); }
        }
    }
}
