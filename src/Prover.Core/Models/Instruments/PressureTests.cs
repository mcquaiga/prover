using Prover.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Models.Instruments
{ 
    public class PressureTest : InstrumentTable
    {
        private decimal? _atmGauge;
        private Instrument _instrument;

        public PressureTest() { }

        public PressureTest(VerificationTest verificationTest) : 
            base(verificationTest.Instrument.Items.CopyItemsByFilter(x => x.IsPressureTest == true))
        {
            VerificationTest = verificationTest;
            _instrument = VerificationTest.Instrument;
            GasGauge = 0;
            AtmosphericGauge = 0;
        }

        [NotMapped]
        public VerificationTest VerificationTest { get; set; }

        public decimal? GasGauge { get; set; }
        public decimal? AtmosphericGauge
        {
            get
            {
                switch (VerificationTest.Instrument.GetTransducerType())
                {
                    case TransducerType.Gauge:
                        return _instrument.EvcAtmosphericPressure();
                    case TransducerType.Absolute:
                        return _atmGauge;
                    default:
                        return _instrument.EvcAtmosphericPressure();
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
                if (Items.EvcPressureFactor() == null) return null;
                if (ActualFactor == 0) return null;
                return Math.Round((decimal)((Items.EvcPressureFactor() - ActualFactor) / ActualFactor) * 100, 2);
            }
        }

        [NotMapped]
        public decimal? ActualFactor
        {
            get
            {
                if (_instrument.EvcBasePressure() == 0) return 0;
                var result = (GasGauge + AtmosphericGauge) / _instrument.EvcBasePressure();
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
