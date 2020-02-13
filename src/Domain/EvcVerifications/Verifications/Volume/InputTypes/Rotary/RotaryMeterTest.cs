using System.Collections.Generic;
using Core.GasCalculations;
using Devices.Core.Items.DriveTypes;
using Shared.Extensions;

namespace Domain.EvcVerifications.Verifications.Volume.InputTypes.Rotary
{
    public class RotaryMeterTest : VerificationEntity
    {
        public RotaryMeterTest(RotaryMeterItems rotaryItems) : base("Rotary Meter Test")
        {
            RotaryItems = rotaryItems;

            if (RotaryItems.MeterType == null)
                throw new KeyNotFoundException(
                    "Could not find a meter type that match the instruments value in item 432.");

            if (RotaryItems.MeterDisplacement == 0)
                ActualValue = RotaryItems.MeterType.MeterDisplacement ?? 0;

            ExpectedValue = RotaryItems.MeterType.MeterDisplacement ?? 0;

            DisplacementPercentError = Calculators.PercentDeviation(ExpectedValue, ActualValue);

            Verified = DisplacementPercentError.IsBetween(Global.METER_DIS_ERROR_THRESHOLD);
        }

        #region Public Properties

        public RotaryMeterItems RotaryItems { get; }

        public decimal ExpectedValue { get; private set; }

        public decimal ActualValue { get; private set; }

        public decimal DisplacementPercentError { get; private set; }

        #endregion

    }
}