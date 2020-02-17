using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Client.Wpf.Screens.Dialogs;
using Client.Wpf.ViewModels.Devices;
using Client.Wpf.Views.Devices;
using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Microsoft.Extensions.Logging;
using MvvmDialogs;
using ReactiveUI;

namespace Client.Wpf.Communications
{
    public class DeviceSessionManager
    {
        private readonly ICommClientFactory _commClientFactory;
        private readonly ICommPortFactory _commPortFactory;
        private readonly ILogger<DeviceSessionManager> _logger;
        private readonly DialogGuy _dialogService;
        private ICommunicationsClient _activeClient;
        private ICommPort _activeCommPort;
        private CancellationTokenSource _cancellationToken;

        public DeviceSessionManager()
        {

        }
        public DeviceSessionManager(ILogger<DeviceSessionManager> logger, DialogGuy dialogService, ICommClientFactory commClientFactory, ICommPortFactory commPortFactory)
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

        //public DeviceSessionManager CreateNew()
        //{
        //    var instance = new DeviceSessionManager(_logger, _dialogService, _commClientFactory, _commPortFactory);

        //    return instance;
        //}

        public async Task DownloadItemFile()
        {
            _activeClient.StatusMessageObservable
                .Subscribe(msg => _logger.Log(msg.LogLevel, msg.ToString()));

            var vm = new SessionDialogViewModel(_activeClient.StatusMessageObservable, _cancellationToken);
            var view = new SessionDialogView();
            view.ViewModel = vm;
            _dialogService.Show(view, vm);


            await Connect();
            var itemValues = await _activeClient.GetItemsAsync();
            await _activeClient.Disconnect();

            Instance = DeviceType.Factory.CreateInstance(itemValues);

            vm.CloseDialog();
            //vm.DialogResult = true;
        }

        public async Task LiveReadItem()
        {
            var obser = new Subject<ItemValue>();
            
            var vm = new SessionDialogViewModel(_activeClient.StatusMessageObservable, _cancellationToken);
            _dialogService.Show<SessionDialogView>(CurrentOwnerViewModel, vm);
            await Connect();
            
            await _activeClient.LiveReadItemValue(26, obser, _cancellationToken.Token);


        }

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

        public async Task<DeviceSessionManager> StartSession(DeviceType deviceType, string commPortName, int baudRate,
            ReactiveObject owner)
        {
            if (SessionInProgress)
                throw new InvalidOperationException("Close the current session before starting a new one.");

            SessionInProgress = true;
            DeviceType = deviceType;
            CurrentOwnerViewModel = owner;
            _activeCommPort = _commPortFactory.Create(commPortName, baudRate);
            _activeClient = _commClientFactory.Create(deviceType, _activeCommPort);
            _cancellationToken = new CancellationTokenSource();

            return this;
        }

        private async Task<DeviceSessionManager> Connect()
        {
            await _activeClient.ConnectAsync();

            return this;
        }
    }
}