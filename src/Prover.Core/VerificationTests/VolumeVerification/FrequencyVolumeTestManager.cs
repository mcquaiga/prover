using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using Prover.Core.VerificationTests.TestActions;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.Core.VerificationTests.VolumeVerification
{
    public sealed class FrequencyVolumeTestManager : VolumeTestManager
    {
        public FrequencyVolumeTestManager(IEventAggregator eventAggregator, ISettingsService settingsService) 
            : base(eventAggregator, settingsService)
        {
        }

        public override void Dispose()
        {
        }     

        public override async Task CompleteTest(ITestActionsManager testActionsManager, CancellationToken ct)
        {
            try
            {
                CommClient.InstrumentType = HoneywellInstrumentTypes.Toc;

                await CommClient.Connect(ct);
                VolumeTest.AfterTestItems = await CommClient.GetVolumeItems();
                if (VolumeTest.VerificationTest.FrequencyTest != null)
                {
                    VolumeTest.VerificationTest.FrequencyTest.PostTestItemValues = await CommClient.GetFrequencyItems();
                }
            }
            finally
            {
                await CommClient.Disconnect();
            }
        }

        public override Task ExecuteSyncTest(CancellationToken ct)
        {
            return null;
        }

        public override async Task PreTest(EvcCommunicationClient commClient, VolumeTest volumeTest, ITestActionsManager testActionsManager, CancellationToken ct)
        {
            CommClient = commClient;
            VolumeTest = volumeTest;

            await CommClient.Connect(ct);
            await testActionsManager.ExecuteValidations(VerificationStep.PreVolumeVerification, CommClient, VolumeTest.Instrument);

            VolumeTest.Items = await CommClient.GetVolumeItems();
            VolumeTest.VerificationTest.FrequencyTest.PreTestItemValues = await CommClient.GetFrequencyItems();
            
            await CommClient.Disconnect();
        }

        public override Task RunTest(CancellationToken ct)
        {
            return null;
        }
    }
}