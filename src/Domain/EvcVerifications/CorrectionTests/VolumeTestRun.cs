using Core.GasCalculations;
using Devices.Core.Items.ItemGroups;
using Shared.Extensions;

namespace Domain.EvcVerifications.CorrectionTests
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

    public class CorrectedVolumeTestRunFactory
    {
        #region Public Properties

        public static CorrectedVolumeTestRunFactory Factory => new CorrectedVolumeTestRunFactory();

        #endregion

        #region Public Methods

        public CorrectedVolumeTestRun Create(IVolumeCorrectedItems startItems, IVolumeCorrectedItems endItems,
            decimal totalCorrectionFactor, decimal uncorrectedInputVolume)
        {
            var test = new CorrectedVolumeTestRun(startItems.CorrectedReading, endItems.CorrectedReading,
                totalCorrectionFactor, uncorrectedInputVolume);
            return test;
        }

        public CorrectedVolumeTestRun Update(
            CorrectedVolumeTestRun testRun, IVolumeCorrectedItems startItems, IVolumeCorrectedItems endItems,
            decimal totalCorrectionFactor, decimal uncorrectedInputVolume)
        {
            testRun.ItemsStart = startItems;
            testRun.ItemsEnd = endItems;

            testRun.Actual = testRun.ItemsEnd.CorrectedReading - testRun.ItemsStart.CorrectedReading;
            testRun.Expected = VolumeCalculator.Corrected(totalCorrectionFactor, uncorrectedInputVolume);

            return testRun;
        }

        #endregion
    }

    public class CorrectedVolumeTestRun : TestRunBase<IVolumeCorrectedItems>
    {
        internal CorrectedVolumeTestRun(decimal startCorrectedReading, decimal endCorrectedReading,
            decimal totalCorrectionFactor, decimal uncorrectedInputVolume)
        {
            Actual = endCorrectedReading - startCorrectedReading;
            Expected = VolumeCalculator.Corrected(totalCorrectionFactor, uncorrectedInputVolume);
        }

        private CorrectedVolumeTestRun()
        {
        }

        #region Public Properties

        public decimal PassTolerance => Global.COR_ERROR_THRESHOLD;

        #endregion

        #region Public Methods

        public override bool HasPassed()
        {
            return Variance(Expected, Actual).IsBetween(PassTolerance);
        }

        #endregion
    }

    public class UncorrectedVolumeTestRun : TestRunBase<IVolumeUncorrectedItems>
    {
        internal UncorrectedVolumeTestRun(decimal startUncorrectedReading, decimal endUncorrectedReading,
            decimal uncorrectedVolumeInput)
        {
            Actual = endUncorrectedReading - startUncorrectedReading;
            Expected = uncorrectedVolumeInput;
        }

        #region Public Properties

        public decimal PassTolerance => Global.UNCOR_ERROR_THRESHOLD;

        #endregion

        #region Public Methods

        public static UncorrectedVolumeTestRun Create(IVolumeUncorrectedItems startItems,
            IVolumeUncorrectedItems endItems, decimal uncorrectedVolumeInput)
        {
            return new UncorrectedVolumeTestRun(startItems.UncorrectedReading, endItems.UncorrectedReading,
                uncorrectedVolumeInput);
        }

        #endregion
    }
}