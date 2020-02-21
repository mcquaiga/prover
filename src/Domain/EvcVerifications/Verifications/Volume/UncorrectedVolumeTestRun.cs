using Core.GasCalculations;
using Devices.Core.Items.ItemGroups;
using Domain.EvcVerifications.Verifications.Volume.InputTypes;

namespace Domain.EvcVerifications.Verifications.Volume
{
    
    public class UncorrectedVolumeTestRun : VerificationTestEntity<VolumeItems, VolumeItems>
    {
        private UncorrectedVolumeTestRun() {}

        #region Public Properties

        public UncorrectedVolumeTestRun(VolumeItems startValues, VolumeItems endValues, decimal expectedValue, decimal actualValue, decimal percentError, bool verified, decimal appliedInput) 
            : base(startValues, endValues, expectedValue, actualValue, percentError, verified)
        {
            AppliedInput = appliedInput;
        }

        public decimal AppliedInput { get; set; }

        #endregion

        //public void Calculate(IVolumeInputType driveInputType)
        //{
        //    ActualValue = VolumeCalculator.TotalVolume(StartValues.UncorrectedReading, EndValues.UncorrectedReading);
        //    ExpectedValue = driveInputType.UnCorrectedInputVolume(AppliedInput);
        //}
    }
}