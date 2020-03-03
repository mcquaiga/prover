using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Screens.Dialogs;
using Client.Desktop.Wpf.ViewModels.Devices;
using Client.Desktop.Wpf.Views.Devices;
using Devices.Communications.Interfaces;
using Devices.Communications.Status;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Microsoft.Extensions.Logging;
using Prover.Shared.IO;
using ReactiveUI;

namespace Client.Desktop.Wpf.Communications
{
    public interface IDeviceSessionManager
    {
        DeviceInstance Device { get; }
        DeviceType DeviceType { get; }
        bool SessionInProgress { get; }

        Task<ICollection<ItemValue>> DownloadCorrectionItems(ICollection<ItemMetadata> items,
            IObserver<ItemReadStatusMessage> itemsObserver = null);

        Task EndSession();

        Task<IEnumerable<ItemValue>> GetItemValues();
        Task LiveReadItem();

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
        Task<DeviceSessionManager> StartSession(DeviceType deviceType, string commPortName, int baudRate,
            ReactiveObject owner);
    }

    public class DeviceSessionManager : DialogViewModel, IDeviceSessionManager
    {
        private readonly ICommClientFactory _commClientFactory;
        private readonly ICommPortFactory _commPortFactory;
        private readonly DialogServiceManager _dialogService;
        private readonly ILogger<DeviceSessionManager> _logger;
        private ICommunicationsClient _activeClient;
        private ICommPort _activeCommPort;
        private CancellationTokenSource _cancellationToken;
        private SessionDialogView _dialogView;
        private SessionDialogViewModel _dialogViewModel;

        public DeviceSessionManager(ILogger<DeviceSessionManager> logger, DialogServiceManager dialogService,
            ICommClientFactory commClientFactory, ICommPortFactory commPortFactory)
        {
            _logger = logger;
            _dialogService = dialogService;
            _commClientFactory = commClientFactory;
            _commPortFactory = commPortFactory;
        }

        public DeviceInstance Device { get; private set; }
        public DeviceType DeviceType { get; private set; }
        public bool SessionInProgress { get; private set; }

        public ReactiveObject CurrentOwnerViewModel { get; set; }

        public async Task<ICollection<ItemValue>> DownloadCorrectionItems(ICollection<ItemMetadata> items,
            IObserver<ItemReadStatusMessage> itemsObserver = null)
        {
            if (itemsObserver != null)
                _activeClient.StatusMessageObservable
                    .OfType<ItemReadStatusMessage>()
                    .Subscribe(itemsObserver);

            await Connect();
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

        public async Task<IEnumerable<ItemValue>> GetItemValues()
        {
            await Connect();
            _dialogViewModel.StatusText = "Downloading items ...";
            var itemValues = await _activeClient.GetItemsAsync();
            await Disconnect();
            return itemValues;
        }

        public async Task LiveReadItem()
        {
            var obser = new Subject<ItemValue>();

            var view = new LiveReadDialogView {ViewModel = this};

            //var vm = new SessionDialogViewModel(_activeClient.StatusMessageObservable, _cancellationToken);
            //await _dialogService.ShowDialog.Execute(vm);
            await Connect();

            await _activeClient.LiveReadItemValue(26, obser, CancellationTokenSource.Token);
        }

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
        public async Task<DeviceSessionManager> StartSession(DeviceType deviceType, string commPortName, int baudRate,
            ReactiveObject owner)
        {
            if (SessionInProgress)
            {
                var answer =
                    await _dialogService.ShowQuestion("Device session already in progress. Start new session?");

                if (answer)
                    await EndSession();
            }

            try
            {
                SessionInProgress = true;
                DeviceType = deviceType;

                CurrentOwnerViewModel = owner;

                _activeCommPort = _commPortFactory.Create(commPortName, baudRate);
                _activeClient = _commClientFactory.Create(deviceType, _activeCommPort);

                CancellationTokenSource = new CancellationTokenSource();

                _activeClient.StatusMessageObservable
                    .Subscribe(msg => _logger.Log(msg.LogLevel, msg.ToString()));

                await SetupDeviceInstance();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured starting session with device.");
                await EndSession();
            }

            return this;
        }

        private async Task Connect()
        {
            if (!_activeClient.IsConnected)
            {
                _dialogViewModel =
                    new SessionDialogViewModel(_activeClient.StatusMessageObservable, CancellationTokenSource)
                    {
                        StatusText = "Connecting ... "
                    };
                await _dialogService.ShowDialog.Execute(_dialogViewModel);

                await _activeClient.ConnectAsync();
            }
        }

        private async Task Disconnect()
        {
            if (_activeClient.IsConnected)
            {
                _dialogViewModel.StatusText = "Disconnecting ...";
                await _activeClient.Disconnect();
                await _dialogService.CloseDialog.Execute();
            }
        }

        private async Task SetupDeviceInstance()
        {
            var itemValues = await GetItemValues();

            Device = DeviceType.Factory.CreateInstance(itemValues);
        }
    }
}