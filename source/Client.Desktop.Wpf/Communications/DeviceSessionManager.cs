using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Communications.Interfaces;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Microsoft.Extensions.Logging;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Shared;

namespace Client.Desktop.Wpf.Communications
{
    public class DeviceSessionManager : IDeviceSessionManager, IActiveDeviceSessionManager
    {
        private readonly Func<DeviceType, ICommunicationsClient> _commClientFactoryFunc;
        private readonly ILogger<DeviceSessionManager> _logger;
        private ICommunicationsClient _activeClient;

        public DeviceSessionManager(ILogger<DeviceSessionManager> logger,
            Func<DeviceType, ICommunicationsClient> commClientFactoryFunc)
        {
            _logger = logger;
            _commClientFactoryFunc = commClientFactoryFunc;
        }

        public DeviceInstance Device { get; private set; }
        public bool SessionInProgress { get; private set; }

        public async Task Connect()
        {
            if (!_activeClient.IsConnected)
            {
                var cancelToken =
                    await DeviceInteractions.Connecting.Handle(_activeClient.StatusMessageObservable);

                cancelToken.Register(() => _activeClient.Cancel());

                await _activeClient.ConnectAsync();
            }
        }

        public async Task Disconnect()
        {
            if (_activeClient.IsConnected)
            {
                await DeviceInteractions.Disconnecting.Handle(this);

                await _activeClient.Disconnect();

                await DeviceInteractions.Unlinked.Handle(this);
            }
        }

        public async Task<ICollection<ItemValue>> DownloadCorrectionItems() =>
            await DownloadCorrectionItems(GetItemsToDownload());

        public async Task<ICollection<ItemValue>> DownloadCorrectionItems(ICollection<ItemMetadata> items)
        {
            await Connect();
            var cancelToken = await DeviceInteractions.DownloadingItems.Handle(this);
            var values = await _activeClient.GetItemsAsync(items);
            await Disconnect();

            return values.ToList();
        }

        public async Task EndSession()
        {
            if (_activeClient != null)
                await _activeClient?.Disconnect();

            _activeClient?.Dispose();

            SessionInProgress = false;
        }

        public ICollection<ItemMetadata> GetItemsToDownload()
        {
            var compType = Device.Composition();
            var items = new List<ItemMetadata>();

            if (Device.HasLivePressure())
                items.AddRange(Device.DeviceType.GetItemMetadata<PressureItems>());

            if (Device.HasLiveTemperature())
                items.AddRange(Device.DeviceType.GetItemMetadata<TemperatureItems>());

            if (Device.HasLiveSuper())
                items.AddRange(Device.DeviceType.GetItemMetadata<SuperFactorItems>());

            return items;
        }

        public async Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<ItemMetadata> itemsToDownload = null)
        {
            await Connect();
            var cancelToken = await DeviceInteractions.DownloadingItems.Handle(this);
            var itemValues = await _activeClient.GetItemsAsync(itemsToDownload);

            return itemValues;
        }

        public async Task<ItemValue> LiveReadItemValue(ItemMetadata item) =>
            await _activeClient.LiveReadItemValue(item);

        public async Task<DeviceInstance> StartSession(DeviceType deviceType)
        {
            if (SessionInProgress)
            {
                var response = await MessageInteractions.ShowYesNo.Handle(
                    "Device session already in progress. Start new session?");

                if (response) await EndSession();
            }

            try
            {
                _activeClient = _commClientFactoryFunc.Invoke(deviceType);

                _activeClient.StatusMessageObservable
                    .Subscribe(msg => _logger.Log(msg.LogLevel, msg.ToString()));

                var itemValues = await GetItemValues();
                Device = deviceType.Factory.CreateInstance(itemValues);
                SessionInProgress = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured starting session with device.");
                await EndSession();
            }

            return Device;
        }

        public async Task<ItemValue> WriteItemValue(ItemMetadata item, string value)
        {
            await Connect();
            var success = await _activeClient.SetItemValue(item.Number, value);

            return success ? ItemValue.Create(item, value) : default;
        }

        public bool Active { get; set; }
    }
}