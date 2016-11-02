using System;
using System.ComponentModel.DataAnnotations;
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
            Items = verificationTest.Instrument.Items.Where(i => i.Metadata.IsTemperatureTest == true);
            VerificationTest = verificationTest;
            VerificationTestId = VerificationTest.Id;

            Gauge = (double) gauge;
        }

        public Guid VerificationTestId { get; set; }

        [Required]
        public VerificationTest VerificationTest { get; set; }

        public double Gauge { get; set; }

        public override decimal? PercentError
        {
            get
            {
                if (Items?.GetItem(ItemCodes.Temperature.Factor) == null) return null;
                if (ActualFactor == null) return null;
                return
                    Math.Round(
                        (decimal)
                        ((Items.GetItem(ItemCodes.Temperature.Factor).NumericValue - ActualFactor)/ActualFactor)*
                        100, 2);
            }
        }

        public override decimal? ActualFactor
        {
            get
            {
                switch (VerificationTest.Instrument.TemperatureUnits())
                {
                    case "K":
                    case "C":
                        return
                            Math.Round(
                                (MetericTempCorrection +
                                 VerificationTest.Instrument.EvcBaseTemperature().GetValueOrDefault(0))/
                                ((decimal) Gauge + MetericTempCorrection), 4);
                    case "R":
                    case "F":
                        return
                            Math.Round(
                                (TempCorrection + VerificationTest.Instrument.EvcBaseTemperature().GetValueOrDefault(0))/
                                ((decimal) Gauge + TempCorrection), 4);
                }

                return 0;
            }
        }

        public override InstrumentType InstrumentType => VerificationTest.Instrument.InstrumentType;
    }
}