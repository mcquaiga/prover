using System.Collections.Generic;
using Devices.Core.Items.DriveTypes;
using Prover.Calculations;
using Prover.Shared.Extensions;

namespace Prover.Application.Models.EvcVerifications.Verifications.Volume.InputTypes.Rotary
{
    public sealed class RotaryMeterTest : VerificationTestEntity<RotaryMeterItems>
    {
        private RotaryMeterTest()
        {
        }

        public RotaryMeterTest(RotaryMeterItems rotaryItems)
        {
            Items = rotaryItems;

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