using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;

namespace Prover.Core.VerificationTests.VolumeVerification
{
    public sealed class ManualVolumeTestManager : VolumeTestManager
    {
        public ManualVolumeTestManager(IEventAggregator eventAggregator, ISettingsService settingsService)
            : base(eventAggregator, settingsService)
        {
        }

        public override async Task PreTest(EvcCommunicationClient commClient, VolumeTest volumeTest, ITestActionsManager testActionsManager, CancellationToken ct)
        {
            await CommClient.Connect(ct);

            VolumeTest.Items = await CommClient.GetVolumeItems();

            if (VolumeTest.VerificationTest.FrequencyTest != null)
            {
                VolumeTest.VerificationTest.FrequencyTest.PreTestItemValues = await CommClient.GetFrequencyItems();
            }

            await CommClient.Disconnect();
        }

        public override async Task RunTest(CancellationToken ct)
        {
            RunningTest = true;
            ResetPulseCounts(VolumeTest);

            await Task.Run(() =>
            {
                while (RunningTest || ct.IsCancellationRequested)
                {

                }
            }, ct);
        }

        public override async Task CompleteTest(ITestActionsManager testActionsManager, CancellationToken ct)
        {
            await Task.Run(async () =>
            {
                try
                {
                    ct.ThrowIfCancellationRequested();
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
            }, ct);
        }

        public override async Task ExecuteSyncTest(CancellationToken ct)
        {
            await Task.Run(() =>
            {
            }, ct);
        }
    }
}

