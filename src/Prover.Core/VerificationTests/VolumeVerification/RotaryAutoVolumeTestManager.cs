namespace Prover.Core.VerificationTests.VolumeVerification
{
    using Caliburn.Micro;
    using Prover.Core.ExternalDevices;
    using Prover.Core.Models.Instruments;
    using Prover.Core.Settings;
    using System.Threading;
    using System.Threading.Tasks;

    public class RotaryAutoVolumeTestManager : AutoVolumeTestManager
    {
        public RotaryAutoVolumeTestManager(IEventAggregator eventAggregator, TachometerService tachComm, ISettingsService settingsService) : base(eventAggregator, tachComm, settingsService)
        {
        }

        protected override void WaitForTestComplete(VolumeTest volumeTest, CancellationToken ct)
        {
            while ((volumeTest.UncPulseCount < volumeTest.DriveType.MaxUncorrectedPulses()) && !ct.IsCancellationRequested)
            {
            }
        }
    }
}