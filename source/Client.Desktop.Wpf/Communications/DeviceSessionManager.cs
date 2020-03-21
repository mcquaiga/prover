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
using Prover.Shared.IO;
using ReactiveUI;

namespace Client.Desktop.Wpf.Communications
{
    public class DeviceSessionManager : IDeviceSessionManager
    {
        private readonly ICommClientFactory _commClientFactory;
        private readonly ICommPortFactory _commPortFactory;
        private readonly ILogger<DeviceSessionManager> _logger;
        private ICommunicationsClient _activeClient;
        private ICommPort _activeCommPort;

        public DeviceSessionManager(ILogger<DeviceSessionManager> logger, ICommClientFactory commClientFactory,
            ICommPortFactory commPortFactory)
        {
            _logger = logger;
            _commClientFactory = commClientFactory;
            _commPortFactory = commPortFactory;
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

            _activeCommPort?.Dispose();
            _activeClient?.Dispose();

            SessionInProgress = false;
        }

        public ICollection<ItemMetadata> GetItemsToDownload()
        {
            var compType = Device.Composition();
            var items = new List<ItemMetadata>();

            if (compType == CompositionType.P || compType == CompositionType.PTZ)
                items.AddRange(Device.DeviceType.GetItemMetadata<PressureItems>());

            if (compType == CompositionType.T || compType == CompositionType.PTZ)
                items.AddRange(Device.DeviceType.GetItemMetadata<TemperatureItems>());

            if (compType == CompositionType.PTZ)
                items.AddRange(Device.DeviceType.GetItemMetadata<SuperFactorItems>());

            return items;
        }

        public async Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<ItemMetadata> itemsToDownload = null)
        {
            await Connect();
            var cancelToken = await DeviceInteractions.DownloadingItems.Handle(this);
            var itemValues = await _activeClient.GetItemsAsync(itemsToDownload);
            await Disconnect();
            return itemValues;
        }

        public async Task<ItemValue> LiveReadItemValue(ItemMetadata item) =>
            await _activeClient.LiveReadItemValue(item);

        /// <summary>
        ///     Configures required resources for device communication
        ///     Tries to connect to device and download full item file
        ///     A device instance will be available in Instance when done
        /// </summary>
        /// <param name="deviceType"></param>
        /// <param name="commPortName"></param>
        /// <param name="baudRate"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public async Task<IDeviceSessionManager> StartSession(DeviceType deviceType, string commPortName, int baudRate,
            ReactiveObject owner)
        {
            if (SessionInProgress)
            {
                var response = await MessageInteractions.ShowYesNo.Handle(
                        "Device session already in progress. Start new session?");

                if (response) await EndSession();
            }

            try
            {
                _activeCommPort = _commPortFactory.Create(commPortName, baudRate);
                _activeClient = _commClientFactory.Create(deviceType, _activeCommPort);

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

            return this;
        }
    }
}