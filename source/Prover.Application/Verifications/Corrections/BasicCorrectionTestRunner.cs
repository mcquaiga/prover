using System.Threading.Tasks;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Extensions;
using Prover.Application.Interfaces;
using Prover.Application.Services.LiveReadCorrections;
using Prover.Application.Verifications.Events;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Corrections;
using ReactiveUI;

namespace Prover.Application.Verifications.Corrections
{
    public class StabilizerCorrectionTestManager : ICorrectionTestsManager
    {
        private readonly IDeviceSessionManager _deviceManager;

        public StabilizerCorrectionTestManager(IDeviceSessionManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        public async Task RunCorrectionTests(VerificationTestPointViewModel test)
        {
            VerificationEvents.CorrectionTest.OnStart.Publish(test);

            await LiveReadCoordinator.StartLiveReading(_deviceManager, test,
                async () =>
                {
                    var values = await _deviceManager.DownloadCorrectionItems();

                    foreach (var correction in test.GetCorrectionTests())
                    {
                        var itemType = correction.GetProperty(nameof(CorrectionTestViewModel<IItemGroup>.Items));

                        itemType?.SetValue(correction, _deviceManager.Device.DeviceType.GetGroupValues(values, itemType.PropertyType));
                    }

                    VerificationEvents.CorrectionTest.OnFinish.Publish(test);
                }, RxApp.MainThreadScheduler);
        }
    }
}