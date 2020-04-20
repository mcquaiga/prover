namespace Prover.Application.Models.EvcVerifications.Verifications.Volume
{
    public class VolumeTestRunFactory
    {
        #region Public Properties

        public static VolumeTestRunFactory Factory => new VolumeTestRunFactory();

        #endregion

        #region Public Methods

        //public CorrectedVolumeTestRun Create(VolumeItems startItems, VolumeItems endItems,
        //    decimal totalCorrectionFactor, decimal uncorrectedInputVolume)
        //{
        //    var test = new CorrectedVolumeTestRun(startItems, endItems, totalCorrectionFactor, uncorrectedInputVolume);
        //    return test;
        //}

        //public UncorrectedVolumeTestRun Create(VolumeItems startItems, VolumeItems endItems, decimal uncorrectedInputVolume)
        //{
        //    var test = new UncorrectedVolumeTestRun(startItems, endItems, uncorrectedInputVolume);
        //    return test;
        //}

        #endregion
    }
}