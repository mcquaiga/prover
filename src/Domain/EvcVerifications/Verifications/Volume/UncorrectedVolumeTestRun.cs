using Devices.Core.Items.ItemGroups;

namespace Domain.EvcVerifications.Verifications.Volume
{
    //public class VolumeTestRunFactory
    //{
    //    #region Public Methods

    //    public VolumeTestRun Create(IVolumeInputType driveType)
    //    {
    //        return new VolumeTestRun(driveType)
    //        {
    //            AppliedInput = 0
    //        };
    //    }

    //    public VolumeTestRun Update(VolumeTestRun testRun, IVolumeItems startItems, IVolumeItems endItems,
    //        decimal appliedInput)
    //    {
    //        return testRun;
    //    }

    //    #endregion
    //}

    //public class VolumeTestRun : AggregateJoin<VerificationTestBaseEntity>, IVerificationTest
    //{
    //    internal VolumeTestRun()
    //    {
    //    }

    //    internal VolumeTestRun(IVolumeInputType driveType)
    //    {
    //        DriveType = driveType;
    //    }

    //    #region Public Properties

    //    public static VolumeTestRunFactory Factory => new VolumeTestRunFactory();

    //    public IVolumeInputType DriveType { get; set; }

    //    public decimal AppliedInput { get; set; }

    //    #endregion

    //    #region Public Methods

    //    public bool HasPassed()
    //    {
    //        return Tests.All(t => t.HasPassed());
    //    }

    //    #endregion
    //}

    public class UncorrectedVolumeTestRun : VerificationTestEntity<VolumeItems, VolumeItems>
    {
        //internal UncorrectedVolumeTestRun(VolumeItems startValues, VolumeItems endValues, decimal uncorrectedVolumeInput) 
        //    : base(startValues, endValues)
        //{
        //    ActualValue = endValues.UncorrectedReading - startValues.UncorrectedReading;
        //    ExpectedValue = uncorrectedVolumeInput;
        //}

        #region Public Properties

        public UncorrectedVolumeTestRun(VolumeItems startValues, VolumeItems endValues, decimal expectedValue, decimal actualValue, decimal percentError, bool verified) 
            : base(startValues, endValues, expectedValue, actualValue, percentError, verified)
        {
        }

        public decimal PassTolerance => Global.UNCOR_ERROR_THRESHOLD;

        #endregion
    }
}