namespace Prover.Core.VerificationTests.VolumeVerification
{
    using Caliburn.Micro;
    using Prover.CommProtocol.Common;
    using Prover.Core.ExternalDevices;
    using Prover.Core.Models.Instruments;
    using System.Threading;
    using System.Threading.Tasks;

    public class MechanicalAutoVolumeTestManager : AutoVolumeTestManager
    {
        public MechanicalAutoVolumeTestManager(IEventAggregator eventAggregator, TachometerService tachComm, Settings.ISettingsService settingsService) : base(eventAggregator, tachComm, settingsService)
        {
        }

        protected override async Task WaitForTestComplete(VolumeTest volumeTest, CancellationToken ct)
        {
            while (await TachometerCommunicator.ReadTach() < 100 && !ct.IsCancellationRequested)
            {
                Thread.Sleep(500);
            }  
        }
    }
}
