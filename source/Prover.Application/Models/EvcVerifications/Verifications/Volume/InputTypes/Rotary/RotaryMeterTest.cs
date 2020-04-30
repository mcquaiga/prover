using Devices.Core.Items.DriveTypes;
using Newtonsoft.Json;
using Prover.Calculations;
using Prover.Shared.Extensions;
using System.Collections.Generic;

namespace Prover.Application.Models.EvcVerifications.Verifications.Volume.InputTypes.Rotary
{
    public sealed class RotaryMeterTest : VerificationTestEntity<RotaryMeterItems>
    {
        private RotaryMeterTest()
        {
        }

        [JsonConstructor]
        public RotaryMeterTest(RotaryMeterItems items)
        {
            Items = items;

            if (Items.MeterType == null)
                throw new KeyNotFoundException(
                    "Could not find a meter type that match the instruments value in item 432.");

            ActualValue = Items.MeterDisplacement != 0
                ? Items.MeterDisplacement
                : Items.MeterType.MeterDisplacement ?? 0;

            ExpectedValue = Items.MeterType.MeterDisplacement ?? 0;

            PercentError = Calculators.PercentDeviation(ExpectedValue, ActualValue);

            Verified = PercentError.IsBetween(Tolerances.METER_DIS_ERROR_THRESHOLD);
        }
    }
}