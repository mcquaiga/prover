using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Extensions;

namespace Prover.Core.Models.Instruments
{
    public sealed class TemperatureTest : BaseVerificationTest
    {
        private const decimal TempCorrection = 459.67m;
        private const decimal MetericTempCorrection = 273.15m;

        public TemperatureTest()
        {
        }

        public TemperatureTest(VerificationTest verificationTest, decimal gauge)
        {
            Items = verificationTest.Instrument.Items.Where(i => i.Metadata.IsTemperatureTest == true).ToList();
            VerificationTest = verificationTest;
            VerificationTestId = VerificationTest.Id;

            Gauge = (double) gauge;
        }

        public double Gauge { get; set; }

        [NotMapped]
        public decimal GaugeFahrenheit => ConvertToFahrenheit((decimal)Gauge, VerificationTest.Instrument.TemperatureUnits());

        public override decimal? ActualFactor
        {
            get
            {
                var result = default(decimal?);
                if (VerificationTest.Instrument.TemperatureUnits() == "K" || VerificationTest.Instrument.TemperatureUnits() == "C")
                {
                    result = 
                        (MetericTempCorrection + VerificationTest.Instrument.EvcBaseTemperature().GetValueOrDefault(0)) /
                             ((decimal) Gauge + MetericTempCorrection);
                }
                else if (VerificationTest.Instrument.TemperatureUnits() == "R" ||
                         VerificationTest.Instrument.TemperatureUnits() == "F")
                {
                    result =
                        (TempCorrection + VerificationTest.Instrument.EvcBaseTemperature().GetValueOrDefault(0)) /
                        ((decimal) Gauge + TempCorrection);
                }

                return result.HasValue ? decimal.Round(result.Value, 4) : default(decimal?);
            }
        }

        public override decimal? EvcFactor => Items.GetItem(ItemCodes.Temperature.Factor)?.NumericValue;
        public override InstrumentType InstrumentType => VerificationTest.Instrument.InstrumentType;

        public static decimal ConvertTo(decimal value, string fromUnit, string toUnit)
        {
            var result = ConvertToFahrenheit(value, fromUnit);

            //convert from F to new unit
            switch (toUnit)
            {
                case "C":
                    result = (result - 32) / 1.8m;
                    break;
                case "K":
                    result = ((result - 32) / 1.8m) + 273.15m;
                    break;
                case "R":
                    result = result + 459.67m;
                    break;
            }

            return decimal.Round(result, 2);
        }

        public static decimal ConvertToFahrenheit(decimal value, string fromUnit)
        {
            switch (fromUnit)
            {
                case "C":
                    value = (value * 1.8m) + 32;
                    break;
                case "K":
                    value = ((value - 273.15m) * 1.8m) + 32;
                    break;
                case "R":
                    value = value - 459.67m;
                    break;
            }

            return decimal.Round(value, 2);
        }
    }
}