using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;

namespace Prover.Core.Models.Instruments
{
    /**
     * 
     * Transducer type = PSIA 
     * 
     **/

    public sealed class PressureTest : BaseVerificationTest
    {
        private const decimal DefaultAtmGauge = 14.73m;

        public PressureTest()
        {
        }

        public PressureTest(VerificationTest verificationTest, decimal gauge)
        {
            Items = verificationTest.Instrument.Items.Where(i => i.Metadata.IsPressureTest == true);
            VerificationTest = verificationTest;
            VerificationTestId = VerificationTest.Id;

            AtmosphericGauge = default(decimal);
            switch (VerificationTest?.Instrument?.Transducer)
            {
                case TransducerType.Gauge:
                    AtmosphericGauge = Items.GetItem(14).NumericValue;
                    break;
                case TransducerType.Absolute:
                    AtmosphericGauge = DefaultAtmGauge;
                    break;
            }
            GasGauge = decimal.Round(gauge, 2);
        }

        public Guid VerificationTestId { get; set; }

        [Required]
        public VerificationTest VerificationTest { get; set; }

        public decimal GasPressure
        {
            get
            {
                var result = GasGauge.GetValueOrDefault(0) + AtmosphericGauge.GetValueOrDefault(0);
                return decimal.Round(result, 2);
            }
        }

        public decimal? GasGauge { get; set; }

        public decimal? AtmosphericGauge { get; set; }

        public override decimal? PercentError
        {
            get
            {
                if ((ActualFactor ?? 0) == 0) return null;

                var result = (EvcFactor - ActualFactor) / ActualFactor * 100;
                return decimal.Round(result ?? 0, 2);
            }
        }

        [NotMapped]
        public decimal EvcFactor => Items?.GetItem(ItemCodes.Pressure.Factor)?.NumericValue ?? 0;

        [NotMapped]
        public override decimal? ActualFactor
        {
            get
            {
                var basePressure = VerificationTest.Instrument.Items.GetItem(ItemCodes.Pressure.Base).NumericValue;
                if (basePressure == 0) return 0;

                var result = GasPressure / basePressure;
                return decimal.Round(result, 4);
            }
        }

        [NotMapped]
        public override InstrumentType InstrumentType => VerificationTest.Instrument.InstrumentType;
    }
}