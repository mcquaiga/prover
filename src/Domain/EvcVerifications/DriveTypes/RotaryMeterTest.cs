using System.Collections.Generic;
using Devices.Core.Items.DriveTypes;
using Domain.EvcVerifications.CorrectionTests;
using Shared.Extensions;

namespace Domain.EvcVerifications.DriveTypes
{
    public class RotaryMeterTest : IVerificationTest
    {
        public IRotaryMeterItems RotaryItems { get; }

        public RotaryMeterTest(IRotaryMeterItems rotaryItems)
        {
            RotaryItems = rotaryItems;
            
            if (RotaryItems.MeterType == null)
                throw new KeyNotFoundException(
                    "Could not find a meter type that match the instruments value in item 432.");

            if (RotaryItems.MeterDisplacement == 0)
                EvcValue = RotaryItems.MeterType.MeterDisplacement ?? 0;

            DisplacementPercentError = CalculateDisplacementPercentError();
        }

        public decimal? Expected => RotaryItems.MeterType.MeterDisplacement;

        public decimal EvcValue { get; set; }

        #region Public Properties

        public decimal DisplacementPercentError { get; set; }

        private decimal CalculateDisplacementPercentError()
        {
            var expected = Expected ?? 0;

            if (expected != 0)
                return Round.Factor(((EvcValue - expected) / expected) * 100m);
            
            return 100m;
        }

        public bool HasPassed() => DisplacementPercentError.IsBetween(Global.METER_DIS_ERROR_THRESHOLD);

        #endregion
    }
}