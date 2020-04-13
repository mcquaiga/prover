using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Extensions;
using Prover.Application.Interfaces;
using Prover.Application.Services.LiveReadCorrections;
using Prover.Application.Verifications.Events;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Corrections;
using ReactiveUI;
using CorrectionEvents = Prover.Application.Verifications.VerificationEvents.CorrectionTests;

namespace Prover.Application.Verifications.Corrections
{
    public abstract class CorrectionTestManagerBase
    {
        
        protected void SetTestItems(VerificationTestPointViewModel test, ICollection<ItemValue> values, DeviceInstance device)
        {
            foreach (var correction in test.GetCorrectionTests())
            {
                var itemType = correction.GetProperty(nameof(CorrectionTestViewModel<IItemGroup>.Items));

                itemType?.SetValue(correction, device.DeviceType.GetGroupValues(values, itemType.PropertyType));
            }
        }
    }

    public class LiveReadStabilizeCorrectionTestManager : CorrectionTestManagerBase, ICorrectionTestsManager
    {
        private readonly IDeviceSessionManager _deviceManager;

        public LiveReadStabilizeCorrectionTestManager(IDeviceSessionManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        public async Task RunCorrectionTests(VerificationTestPointViewModel test)
        {
           

            await LiveReadCoordinator.StartLiveReading(_deviceManager, test,
                async () =>
                {
                    await CorrectionEvents.OnLiveReadComplete.Publish(test);

                    await CorrectionEvents.BeforeDownload.Publish(test);
                    
                    var values = await _deviceManager.DownloadCorrectionItems();
                    SetTestItems(test, values, _deviceManager.Device);

                    await CorrectionEvents.OnComplete.Publish(test);

                }, RxApp.MainThreadScheduler);
        }
    }

    public class DownloadItemsOnlyCorrectionTestManager : ICorrectionTestsManager
    {
        /// <inheritdoc />
        public Task RunCorrectionTests(VerificationTestPointViewModel test) => throw new System.NotImplementedException();
    }
}