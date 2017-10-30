using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Extensions;

namespace Prover.Core.Models.Instruments
{
    /**
     * 
     * Transducer type = PSIA 
     * 
     **/

    public sealed class PressureTest : BaseVerificationTest
    {
        private const decimal DefaultAtmGauge = 0m;

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
                    AtmosphericGauge = null;
                    GasGauge = TotalGauge - (AtmosphericGauge ?? 0);
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
                var result = GasGauge.GetValueOrDefault(0) + AtmosphericGauge.GetValueOrDefault(0);
                return decimal.Round(result, 4);
            }
        }

        [NotMapped]
        public decimal GasPressurePsi => ConvertToPsi(GasPressure, VerificationTest.Instrument.PressureUnits());

        public static decimal ConvertTo(decimal value, string fromUnit, string toUnit)
        {
            var result = ConvertToPsi(value, fromUnit);

            switch (toUnit.ToLower())
            {
                case "bar":
                    result = value * (6.894757m * (10 ^ -2));
                    break;
                case "inwc":
                    result = value * 27.68067m;
                    break;
                case "kgcm2":
                    result = value * (7.030696m * (10 ^ -2));
                    break;
                case "kpa":
                    result = value * 6.894757m;
                    break;
                case "mbar":
                    result = value * (6.894757m * (10 ^ 1));
                    break;
                case "mpa":
                    result = value * (6.894757m * (10 ^ -3));
                    break;
                case "inhg":
                    result = value * 2.03602m;
                    break;
                case "mmhg":
                    result = value * 51.71492m;
                    break;
            }

            return decimal.Round(result, 2);
        }

        public static decimal ConvertToPsi(decimal value, string fromUnit)
        {
            var result = 0.0m;
            switch (fromUnit.ToLower())
            {
                case "bar":
                    result = value / (6.894757m * (10 ^ -2));
                    break;
                case "inwc":
                    result = value / 27.68067m;
                    break;
                case "kgcm2":
                    result = value / (7.030696m * (10 ^ -2));
                    break;
                case "kpa":
                    result = value / 6.894757m;
                    break;
                case "mbar":
                    result = value / (6.894757m * (10 ^ 1));
                    break;
                case "mpa":
                    result = value / (6.894757m * (10 ^ -3));
                    break;
                case "inhg":
                    result = value / 2.03602m;
                    break;
                case "mmhg":
                    result = value / 51.71492m;
                    break;                
            }

            return decimal.Round(result, 2);
        }

        public decimal? GasGauge { get; set; }

        [NotMapped]
        public decimal? GasGaugePsi 
            => GasGauge.HasValue ? ConvertToPsi(GasGauge.Value, VerificationTest.Instrument.PressureUnits()) : default(decimal?);

        public decimal? AtmosphericGauge { get; set; }

        [NotMapped]
        public override decimal? EvcFactor => Items?.GetItem(ItemCodes.Pressure.Factor)?.NumericValue ?? 0;

        [NotMapped]
        public override decimal? ActualFactor
        {
            get
            {
                var basePressure = VerificationTest.Instrument.Items.GetItem(ItemCodes.Pressure.Base).NumericValue;
                if (basePressure == 0) return 0;
             
                return decimal.Round(GasPressure / basePressure, 4);
            }
        }

        [NotMapped]
        public override InstrumentType InstrumentType => VerificationTest.Instrument.InstrumentType;
    }
}