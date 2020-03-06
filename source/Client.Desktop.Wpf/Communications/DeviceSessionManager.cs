using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Screens.Dialogs;
using Client.Desktop.Wpf.ViewModels.Devices;
using Client.Desktop.Wpf.Views.Devices;
using Client.Desktop.Wpf.Views.Verifications.Dialogs;
using Devices.Communications.Interfaces;
using Devices.Communications.Status;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using DynamicData;
using Microsoft.Extensions.Logging;
using Prover.Application.ViewModels;
using Prover.Shared.IO;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

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
        ReactiveCommand<Unit, Unit> RequestCancellation { get; set; }
        Task LiveReadItem(ICollection<ItemMetadata> items, CancellationTokenSource cts);
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
        private CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        private SessionDialogView _dialogView;
        private SessionDialogViewModel _dialogViewModel;

        public DeviceSessionManager(ILogger<DeviceSessionManager> logger, DialogServiceManager dialogService,
            ICommClientFactory commClientFactory, ICommPortFactory commPortFactory)
        {
            _logger = logger;
            _dialogService = dialogService;
            _commClientFactory = commClientFactory;
            _commPortFactory = commPortFactory;
            
            RequestCancellation = ReactiveCommand.Create(() => _cancellationToken.Cancel());
        }

        public ReactiveCommand<Unit, Unit> RequestCancellation { get; set; }

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
            _cancellationToken.Dispose();

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

        public async Task LiveReadItem(ICollection<ItemMetadata> items, CancellationTokenSource cts)
        {
            var cleanup = new CompositeDisposable();
            _cancellationToken = cts ?? _cancellationToken;
            _cancellationToken.Token.Register(async () => await StopLiveReading());
            
            await Connect();
            await _dialogService.CloseDialog.Execute();
            
            var live = SetupLiveReadObservable();
            
            await ShowLiveReadDialog();


            cleanup.Add(live.Connect());
            
            async Task StopLiveReading()
            {
                cleanup.Dispose();
                await Task.Delay(500);

                await Disconnect();

                await CloseCommand.Execute();
                _cancellationToken = new CancellationTokenSource();
            }

            async Task ShowLiveReadDialog()
            {
                var liveDialog = new LiveReadDialogView { ViewModel = this };
                await _dialogService.ShowDialogView.Execute(liveDialog);
            }

            IConnectableObservable<IChangeSet<ItemValue, ItemMetadata>> SetupLiveReadObservable()
            {
                var liveReads = StartLiveRead(items).Publish();

                liveReads.Bind(out var reads)
                    .Subscribe();
                LiveReadingUpdates = reads;

                LiveReadUpdates = liveReads.AsObservableCache();

                LiveUpdates = LiveReadUpdates
                    .Connect()
                    .RemoveKey()
                    .AsObservableList();
                    //.LogDebug(x => $"{x.Metadata.ShortDescription} - {x.DecimalValue()}")
                    //.Subscribe();



                return liveReads;
            }
        }

        public IObservableList<ItemValue> LiveUpdates { get; set; }

        public ReadOnlyObservableCollection<ItemValue> LiveReadingUpdates { get; set; }

        public IObservableCache<ItemValue, ItemMetadata> LiveReadUpdates { get; set; }
        

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
                _cancellationToken = new CancellationTokenSource();

                SessionInProgress = true;
                DeviceType = deviceType;

                CurrentOwnerViewModel = owner;

                _activeCommPort = _commPortFactory.Create(commPortName, baudRate);
                _activeClient = _commClientFactory.Create(deviceType, _activeCommPort);

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

        protected async Task Connect()
        {
            if (!_activeClient.IsConnected)
            {
                _dialogViewModel = new SessionDialogViewModel(_activeClient.StatusMessageObservable, _cancellationToken)
                    {
                        StatusText = "Connecting ... "
                    };
                await _dialogService.ShowDialog.Execute(_dialogViewModel);

                await _activeClient.ConnectAsync();
            }
        }

        protected async Task Disconnect()
        {
            if (_activeClient.IsConnected)
            {
                _dialogViewModel.StatusText = "Disconnecting ...";
                await _activeClient.Disconnect();
                await _dialogService.CloseDialog.Execute();
            }
        }

        protected IObservable<IChangeSet<ItemValue, ItemMetadata>> StartLiveRead(ICollection<ItemMetadata> items)
        {
            return ObservableChangeSet.Create<ItemValue, ItemMetadata>(cache =>
                {
                    var liveRead = TaskPoolScheduler.Default.SchedulePeriodic(TimeSpan.FromMilliseconds(1000), () =>
                    {
                        items.ForEach(async i =>
                        {
                            var value = await _activeClient.LiveReadItemValue(i);
                            cache.AddOrUpdate(value);
                        });
                    });

                    return liveRead;
                }, value => value.Metadata);
        }

        private async Task SetupDeviceInstance()
        {
            var itemValues = await GetItemValues();

            Device = DeviceType.Factory.CreateInstance(itemValues);
        }
    }
}