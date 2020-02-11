using Core.GasCalculations;
using Devices.Core.Items.ItemGroups;

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

        public CorrectionTest Create(IVolumeCorrectedItems startItems, IVolumeCorrectedItems endItems,
            decimal totalCorrectionFactor, decimal uncorrectedInputVolume)
        {
            var test = new CorrectedVolumeTestRun(startItems.CorrectedReading, endItems.CorrectedReading,
                totalCorrectionFactor, uncorrectedInputVolume);
            return test;
        }

        public CorrectionTest Update(
            CorrectionTest testRun, IVolumeCorrectedItems startItems, IVolumeCorrectedItems endItems,
            decimal totalCorrectionFactor, decimal uncorrectedInputVolume)
        {
            //testRun.ItemsStart = startItems;
            //testRun.ItemsEnd = endItems;

            testRun.ActualValue = endItems.CorrectedReading - startItems.CorrectedReading;
            testRun.ExpectedValue = VolumeCalculator.Corrected(totalCorrectionFactor, uncorrectedInputVolume);

            return testRun;
        }

        #endregion
    }

    public class CorrectedVolumeTestRun : CorrectionTest
    {
        internal CorrectedVolumeTestRun(decimal startCorrectedReading, decimal endCorrectedReading,
            decimal totalCorrectionFactor, decimal uncorrectedInputVolume) 
            : base(CorrectionFactorTestType.CorrectedVolume)
        {
            ActualValue = endCorrectedReading - startCorrectedReading;
            ExpectedValue = VolumeCalculator.Corrected(totalCorrectionFactor, uncorrectedInputVolume);
        }

        #region Public Properties

        public override decimal PassTolerance => Global.COR_ERROR_THRESHOLD;

        #endregion
    }

    public class UncorrectedVolumeTestRun : CorrectionTest
    {
        internal UncorrectedVolumeTestRun(decimal startUncorrectedReading, decimal endUncorrectedReading,
            decimal uncorrectedVolumeInput) : base(CorrectionFactorTestType.UncorrectedVolume)
        {
            ActualValue = endUncorrectedReading - startUncorrectedReading;
            ExpectedValue = uncorrectedVolumeInput;
        }

        #region Public Properties

        public override decimal PassTolerance => Global.UNCOR_ERROR_THRESHOLD;

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