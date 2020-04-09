using System.Threading.Tasks;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Extensions;
using Prover.Application.Interfaces;
using Prover.Application.Services.LiveReadCorrections;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Corrections;
using ReactiveUI;

namespace Prover.Application.VerificationManagers.Corrections
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
            await LiveReadCoordinator.StartLiveReading(_deviceManager, test,
                async () =>
                {
                    var values = await _deviceManager.DownloadCorrectionItems();

                    //test.UpdateItemValues(values);

                    foreach (var correction in test.GetCorrectionTests())
                    {
                        var itemType = correction.GetProperty(nameof(CorrectionTestViewModel<IItemGroup>.Items));

                        itemType?.SetValue(correction, _deviceManager.Device.DeviceType.GetGroupValues(values, itemType.PropertyType));
                    }
                }, RxApp.MainThreadScheduler);
        }
    }
}