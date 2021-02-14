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

			Status.OnNext("Completing volume test...");
			ct.ThrowIfCancellationRequested();

			await Task.Delay(250);

			try {
				await CommClient.Connect(ct);

				VolumeTest.AfterTestItems = await CommClient.GetVolumeItems();

				//Frequency only instruments
				if (VolumeTest.VerificationTest.FrequencyTest != null) {
					VolumeTest.VerificationTest.FrequencyTest.PostTestItemValues = await CommClient.GetFrequencyItems();
				}

				await testActionsManager.ExecuteValidations(TestActions.VerificationStep.PostVolumeVerification, CommClient, VolumeTest.Instrument);
			}
			finally {
				await CommClient.Disconnect();
			}
		}

		public override Task ExecuteSyncTest(CancellationToken ct) {
			return Task.CompletedTask;
		}

		public override async Task PreTest(EvcCommunicationClient commClient, VolumeTest volumeTest, ITestActionsManager testActionsManager, CancellationToken ct) {

			await Task.Run(async () => {
				CommClient = commClient;
				VolumeTest = volumeTest;

				await CommClient.Connect(ct);

				await testActionsManager.ExecuteValidations(TestActions.VerificationStep.PreVolumeVerification, CommClient, VolumeTest.Instrument);

				VolumeTest.Items = await CommClient.GetVolumeItems();

				if (VolumeTest.VerificationTest.FrequencyTest != null) {
					VolumeTest.VerificationTest.FrequencyTest.PreTestItemValues = await CommClient.GetFrequencyItems();
				}

				await CommClient.Disconnect();
			});

		}

		public override Task RunTest(CancellationToken ct) {
			RunningTest = true;
			ResetPulseCounts(VolumeTest);

			return Task.CompletedTask;
			//return Task.Run(() =>
			// {
			//     while (RunningTest || ct.IsCancellationRequested)
			//     {
			//     }
			// }, ct);
		}
	}
}