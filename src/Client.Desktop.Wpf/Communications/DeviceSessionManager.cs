using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.Wpf.ViewModels.Devices;
using Client.Wpf.Screens.Dialogs;
using Client.Wpf.Views.Devices;
using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Communications.Status;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace Client.Wpf.Communications
{
    public class DeviceSessionManager
    {
        private readonly ICommClientFactory _commClientFactory;
        private readonly ICommPortFactory _commPortFactory;
        private readonly DialogServiceManager _dialogService;
        private readonly ILogger<DeviceSessionManager> _logger;
        private ICommunicationsClient _activeClient;
        private ICommPort _activeCommPort;
        private CancellationTokenSource _cancellationToken;
        private SessionDialogViewModel _dialogViewModel;
        private SessionDialogView _dialogView;

        public DeviceSessionManager()
        {
        }

        public DeviceSessionManager(ILogger<DeviceSessionManager> logger, DialogServiceManager dialogService,
            ICommClientFactory commClientFactory, ICommPortFactory commPortFactory)
        {
            _logger = logger;
            _dialogService = dialogService;
            _commClientFactory = commClientFactory;
            _commPortFactory = commPortFactory;
        }

        public DeviceInstance Instance { get; private set; }
        public DeviceType DeviceType { get; private set; }
        public bool SessionInProgress { get; private set; }

        public ReactiveObject CurrentOwnerViewModel { get; set; }

        public async Task EndSession()
        {
            if (_activeClient != null)
                await _activeClient?.Disconnect();

            _activeCommPort?.Dispose();
            _activeClient?.Dispose();

            SessionInProgress = false;

            //var newSession = CreateNew();
            //this = newSession;
            //return this;
        }

        public async Task LiveReadItem()
        {
            var obser = new Subject<ItemValue>();

            var vm = new SessionDialogViewModel(_activeClient.StatusMessageObservable, _cancellationToken);
            _dialogService.ShowDialog.Execute(vm);
            await Connect();

            await _activeClient.LiveReadItemValue(26, obser, _cancellationToken.Token);
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
                throw new InvalidOperationException("Close the current session before starting a new one.");

            try
            {
                SessionInProgress = true;
                DeviceType = deviceType;
                CurrentOwnerViewModel = owner;
                _activeCommPort = _commPortFactory.Create(commPortName, baudRate);
                _activeClient = _commClientFactory.Create(deviceType, _activeCommPort);
                _cancellationToken = new CancellationTokenSource();

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

        public async Task<ICollection<ItemValue>> DownloadCorrectionItems(ICollection<ItemMetadata> items, IObserver<ItemReadStatusMessage> itemsObserver = null)
        {
            if (itemsObserver != null)
            {
                _activeClient.StatusMessageObservable
                    .OfType<ItemReadStatusMessage>()
                    .Subscribe(itemsObserver);
            }

            await Connect();
            var values = await _activeClient.GetItemsAsync(items);
            await Disconnect();

            return values.ToList();
        }

        private async Task<DeviceSessionManager> Connect()
        {
            if (!_activeClient.IsConnected)
            {
                _dialogViewModel = new SessionDialogViewModel(_activeClient.StatusMessageObservable, _cancellationToken);
                await _dialogService.ShowDialog.Execute(_dialogViewModel);

                await _activeClient.ConnectAsync();
            }

            return this;
        }

        private async Task<DeviceSessionManager> Disconnect()
        {
            if (_activeClient.IsConnected)
            {
                _dialogViewModel.TitleText = "Disconnecting";
                await _activeClient.Disconnect();
                _dialogService.CloseDialog.Execute();
            }
            return this;
        }

        private async Task SetupDeviceInstance()
        {
            await Connect();

            _dialogViewModel.TitleText = "Downloading items...";
            var itemValues = await _activeClient.GetItemsAsync();
            
            await Disconnect();

            Instance = DeviceType.Factory.CreateInstance(itemValues);
        }

    }
}