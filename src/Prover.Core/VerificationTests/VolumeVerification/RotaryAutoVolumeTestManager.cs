namespace Prover.Core.VerificationTests.VolumeVerification
{
    using Caliburn.Micro;
    using Prover.Core.ExternalDevices;
    using Prover.Core.Models.Instruments;
    using System.Threading;
    using System.Threading.Tasks;

    public class RotaryAutoVolumeTestManager : AutoVolumeTestManager
    {
        public RotaryAutoVolumeTestManager(IEventAggregator eventAggregator, CommProtocol.Common.EvcCommunicationClient commClient, VolumeTest volumeTest, TachometerService tachComm, Settings.ISettingsService settingsService) : base(eventAggregator, commClient, volumeTest, tachComm, settingsService)
        {
        }

        protected override async Task WaitForTestComplete(VolumeTest volumeTest, CancellationToken ct)
        {         
           await Task.Run(() =>
           {
               while ((volumeTest.UncPulseCount < volumeTest.DriveType.MaxUncorrectedPulses()) && !ct.IsCancellationRequested)
               {
               }
               OutputBoard.StopMotor();
               Log.Debug($"Test stopped at {volumeTest.UncPulseCount}");
           });           
        }
    }
}
