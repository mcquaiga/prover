using Prover.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;

namespace Prover.Core.Models.Instruments
{ 
    public sealed class PressureTest : BaseVerificationTest
    {
        private const decimal DefaultAtmGauge = 14.0m;

        public PressureTest(VerificationTest verificationTest, decimal gauge) : base()
        {
            VerificationTest = verificationTest;
            VerificationTestId = VerificationTest.Id;
            GasGauge = decimal.Round(gauge, 2);
            AtmosphericGauge = decimal.Round(DefaultAtmGauge, 2);
        }
          
        public Guid VerificationTestId { get; set; }
        [Required]
        public VerificationTest VerificationTest { get; set; }

        public decimal? GasPressure
        {
            get
            {
                if (VerificationTest == null) return null;

                var result = 0.0m;
                switch ((TransducerType)VerificationTest.Instrument.Items.GetItem(ItemCodes.Pressure.TransducerType).NumericValue)
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
                if (Items.GetItem(ItemCodes.Pressure.Factor) == null) return null;
                if (ActualFactor == 0 || ActualFactor == null) return null;
                return Math.Round((decimal)((Items.GetItem(ItemCodes.Pressure.Factor).NumericValue - ActualFactor) / ActualFactor) * 100, 2);
            }
        }

        [NotMapped]
        public override decimal? ActualFactor
        {
            get
            {
                if (VerificationTest.Instrument.Items.GetItem(ItemCodes.Pressure.Base).NumericValue == 0) return 0;
                var result = GasPressure / VerificationTest.Instrument.Items.GetItem(ItemCodes.Pressure.Base).NumericValue;
                return result.HasValue ? decimal.Round(result.Value, 4) : 0;
            }
        }

        [NotMapped]
        public override InstrumentType InstrumentType => VerificationTest.Instrument.InstrumentType;
    }
}
