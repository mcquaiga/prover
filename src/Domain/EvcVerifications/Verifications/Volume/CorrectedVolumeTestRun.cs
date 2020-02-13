using Devices.Core.Items.ItemGroups;

namespace Domain.EvcVerifications.Verifications.Volume
{
    public class CorrectedVolumeTestRun : VerificationTestEntity<VolumeItems, VolumeItems>
    {
        //internal CorrectedVolumeTestRun(VolumeItems startItems, VolumeItems endItems,
        //    decimal totalCorrectionFactor, decimal uncorrectedInputVolume)
        //    : base(VerificationTestType.CorrectedVolume, startItems, endItems)
        //{
        //    ActualValue = endItems.CorrectedReading - startItems.CorrectedReading;
        //    ExpectedValue = VolumeCalculator.Corrected(totalCorrectionFactor, uncorrectedInputVolume);
        //}



        #region Public Properties

        public CorrectedVolumeTestRun(VolumeItems startValues, VolumeItems endValues, decimal expectedValue, decimal actualValue, decimal percentError, bool verified) 
            : base(startValues, endValues, expectedValue, actualValue, percentError, verified)
        {
        }

        public decimal PassTolerance => Global.COR_ERROR_THRESHOLD;

        #endregion
    }
}