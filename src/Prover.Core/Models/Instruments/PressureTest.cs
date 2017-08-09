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
        private const decimal DefaultAtmGauge = 14.0m;

        public PressureTest()
        {
        }

        public PressureTest(VerificationTest verificationTest, decimal gauge)
        {
            Items = verificationTest.Instrument.Items.Where(i => i.Metadata.IsPressureTest == true).ToList();
            VerificationTest = verificationTest;
            VerificationTestId = VerificationTest.Id;

            _totalGauge = decimal.Round(gauge, 2);
            AtmosphericGauge = default(decimal?);

            switch (VerificationTest?.Instrument?.Transducer)
            {
                case TransducerType.Gauge:
                    GasGauge = TotalGauge;
                    AtmosphericGauge = VerificationTest.Instrument.Items.GetItem(14).NumericValue;
                    break;
                case TransducerType.Absolute:
                    AtmosphericGauge = DefaultAtmGauge;
                    GasGauge = TotalGauge - AtmosphericGauge;
                    break;
            }
        }


        private readonly decimal? _totalGauge;

        [NotMapped]
        public decimal TotalGauge => _totalGauge ?? 0;

        public decimal GasPressure
        {
            get
            {

                var result = 0m;
                switch (VerificationTest?.Instrument?.Transducer)
                {
                    case TransducerType.Gauge:
                        result = GasGauge.GetValueOrDefault(0);
                        break;
                    case TransducerType.Absolute:
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

                var gasPressure = GasGauge.GetValueOrDefault(0) + AtmosphericGauge.GetValueOrDefault(0);

                return decimal.Round(gasPressure / basePressure, 4);
            }
        }

        [NotMapped]
        public override InstrumentType InstrumentType => VerificationTest.Instrument.InstrumentType;
    }
}