using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Devices.Communications.Interfaces;
using Devices.Communications.Status;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using Prover.Application.FileLoader;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using ReactiveUI;

namespace Client.Desktop.Wpf.Communications
{
    public class FileCommunicationsClient : ICommunicationsClient
    {
        private readonly CompositeDisposable _cleanup;

        private readonly IDeviceRepository _deviceRepository;

        private readonly Dictionary<ItemMetadata, decimal> _mockedLiveReadValues =
            new Dictionary<ItemMetadata, decimal>();

        private readonly ISubject<StatusMessage> _statusSubject = new Subject<StatusMessage>();
        private ItemAndTestFile _itemFile;

        public FileCommunicationsClient(IDeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;

            var status = _statusSubject.Publish();
            StatusMessageObservable = status.AsObservable();

            _cleanup = new CompositeDisposable(status.Connect());
        }

        public bool IsConnected { get; private set; }

        public IObservable<StatusMessage> StatusMessageObservable { get; }

        public string FilePath { get; private set; }

        public void Cancel()
        {
        }

        public async Task ConnectAsync(int retryAttempts = 10, TimeSpan? timeout = null)
        {
            if (string.IsNullOrEmpty(FilePath))
                throw new NullReferenceException("ItemFile cannot be null. Call SetItemFileAndTestDefinition first.");

            _itemFile = await ItemLoader.LoadFromFile(_deviceRepository, FilePath);

            IsConnected = true;
        }

        public async Task Disconnect()
        {
            IsConnected = false;
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _cleanup?.Dispose();
        }

        public async Task<IEnumerable<ItemValue>> GetItemsAsync(IEnumerable<ItemMetadata> itemNumbers)
        {
            if (_mockedLiveReadValues.Any())
                return await GetItemsAfterLiveRead();

            if (itemNumbers == null)
                return _itemFile.Device.Values;

            return _itemFile.Device.Values.Where(i => itemNumbers.Contains(i.Metadata));
        }

        public async Task<ItemValue> LiveReadItemValue(ItemMetadata itemNumber)
        {
            if (!_mockedLiveReadValues.ContainsKey(itemNumber))
            {
                var value =
                    await MessageInteractions.GetInputNumber.Handle(
                            $"Enter simulating value for {itemNumber.Description} - #{itemNumber.Number}:")
                        .ObserveOn(RxApp.MainThreadScheduler);

                _mockedLiveReadValues.Add(itemNumber, value);
            }

            return ItemValue.Create(itemNumber, _mockedLiveReadValues[itemNumber]);
        }

        public void SetFilePath(string filePath)
        {
            if (File.Exists(filePath))
                FilePath = filePath;
        }

        public Task<bool> SetItemValue(int itemNumber, string value) => throw new NotImplementedException();

        private async Task<IEnumerable<ItemValue>> GetItemsAfterLiveRead()
        {
            var testNumber = await MessageInteractions.GetInputInteger.Handle("Enter test number to load (1, 2, 3):");
            _mockedLiveReadValues.Clear();
            return _itemFile.TemperatureTests[testNumber].ToList().Union(_itemFile.PressureTests[testNumber]).ToList();
        }
    }


    public class FileDeviceSessionManager : IDeviceSessionManager, IActiveDeviceSessionManager
    {
        private readonly IDeviceRepository _deviceRepository;
        private ItemAndTestFile _itemFile;

        public FileDeviceSessionManager(IDeviceRepository deviceRepository) => _deviceRepository = deviceRepository;

        public DeviceInstance Device { get; private set; }
        public bool SessionInProgress { get; private set; }
        public bool Active { get; set; }

        public string FilePath { get; private set; }

        public async Task Connect()
        {
            SessionInProgress = true;
            await Task.CompletedTask;
        }

        public Task Disconnect() => throw new NotImplementedException();

        public async Task<ICollection<ItemValue>> DownloadCorrectionItems()
        {
            var testNumber = await MessageInteractions.GetInputInteger.Handle("Enter test number to load (1, 2, 3):");

            return _itemFile.TemperatureTests[testNumber].ToList()
                .Union(_itemFile.PressureTests[testNumber]).ToList();
        }

        public Task<ICollection<ItemValue>> DownloadCorrectionItems(ICollection<ItemMetadata> items) =>
            throw new NotImplementedException();

        public Task EndSession() => throw new NotImplementedException();

        public ICollection<ItemMetadata> GetItemsToDownload() => throw new NotImplementedException();

        public Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<ItemMetadata> itemsToDownload = null) =>
            throw new NotImplementedException();

        public Task<ItemValue> LiveReadItemValue(ItemMetadata item) => throw new NotImplementedException();

        public void SetFilePath(string filePath)
        {
            if (File.Exists(filePath))
                FilePath = filePath;
        }

        public void SetItemFileAndTestDefinition(ItemAndTestFile testFile)
        {
            _itemFile = testFile;
        }

        public async Task<DeviceInstance> StartSession(DeviceType deviceType)
        {
            Active = true;
            SessionInProgress = true;
            if (string.IsNullOrEmpty(FilePath))
                throw new NullReferenceException("ItemFile cannot be null. Call SetItemFileAndTestDefinition first.");

            _itemFile = await ItemLoader.LoadFromFile(_deviceRepository, FilePath);
            Device = _itemFile.Device;

            return Device;
        }

        public Task<ItemValue> WriteItemValue(ItemMetadata item, string value) => throw new NotImplementedException();
    }
}