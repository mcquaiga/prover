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

        protected override async Task WaitForTestComplete(VolumeTest volumeTest, CancellationToken ct)
        {
            await Task.Run(() =>
            {
                //volumeTest.DriveType.MaxUncorrectedPulses()
                while ((volumeTest.UncPulseCount < volumeTest.DriveType.MaxUncorrectedPulses()) && !ct.IsCancellationRequested)
                {
                }
                OutputBoard.StopMotor();
                Log.Debug($"Test stopped at {volumeTest.UncPulseCount}");
            });
        }
    }
}