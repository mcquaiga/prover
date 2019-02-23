namespace Prover.Core.VerificationTests.VolumeVerification
{
    using Caliburn.Micro;
    using Prover.Core.ExternalDevices;
    using Prover.Core.Models.Instruments;
    using Prover.Core.Settings;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class MechanicalAutoVolumeTestManager : AutoVolumeTestManager
    {
        public MechanicalAutoVolumeTestManager(IEventAggregator eventAggregator, TachometerService tachComm, ISettingsService settingsService) : base(eventAggregator, tachComm, settingsService)
        {
        }      

        protected override async Task WaitForTestComplete(VolumeTest volumeTest, CancellationToken ct)
        {           
            await Task.Run(async () =>
            {
                while (await TachometerCommunicator.ReadTach() < 100 && !ct.IsCancellationRequested)
                {
                    Thread.Sleep(500);
                }  
            });            
        }
    }
}
