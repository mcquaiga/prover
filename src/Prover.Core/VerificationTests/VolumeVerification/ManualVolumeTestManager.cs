using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.Core.VerificationTests.VolumeVerification {
	public sealed class ManualVolumeTestManager : VolumeTestManager {
		public ManualVolumeTestManager(IEventAggregator eventAggregator, ISettingsService settingsService)
			: base(eventAggregator, settingsService) {
		}

		public override async Task CompleteTest(ITestActionsManager testActionsManager, CancellationToken ct) {
			ct.ThrowIfCancellationRequested();
			try {
				await CommClient.Connect(ct);

				VolumeTest.AfterTestItems = await CommClient.GetVolumeItems();

				//Frequency only instruments
				if (VolumeTest.VerificationTest.FrequencyTest != null) {
					VolumeTest.VerificationTest.FrequencyTest.PostTestItemValues = await CommClient.GetFrequencyItems();
				}
			}
			finally {
				await CommClient.Disconnect();
			}
		}

		public override Task ExecuteSyncTest(CancellationToken ct) {
			return Task.Run(() => {
			}, ct);
		}

		public override async Task PreTest(EvcCommunicationClient commClient, VolumeTest volumeTest, ITestActionsManager testActionsManager, CancellationToken ct) {
			await CommClient.Connect(ct);

			VolumeTest.Items = await CommClient.GetVolumeItems();

			if (VolumeTest.VerificationTest.FrequencyTest != null) {
				VolumeTest.VerificationTest.FrequencyTest.PreTestItemValues = await CommClient.GetFrequencyItems();
			}

			await CommClient.Disconnect();
		}

		public override Task RunTest(CancellationToken ct) {
			RunningTest = true;
			ResetPulseCounts(VolumeTest);

			return Task.Run(() => {
				while (RunningTest || ct.IsCancellationRequested) {
				}
			}, ct);
		}
	}
}