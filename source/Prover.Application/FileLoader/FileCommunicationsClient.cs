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
using Devices.Core.Items;
using Devices.Core.Repository;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using ReactiveUI;

namespace Prover.Application.FileLoader
{
    public class FileCommunicationsClient : ICommunicationsClient
    {
        private readonly CompositeDisposable _cleanup;

        private readonly IDeviceRepository _deviceRepository;

        private readonly Dictionary<ItemMetadata, decimal> _mockedLiveReadValues =
            new Dictionary<ItemMetadata, decimal>();

        private readonly ISubject<StatusMessage> _statusSubject = new Subject<StatusMessage>();
        private ItemAndTestFile _itemFile;

        public FileCommunicationsClient(IDeviceRepository deviceRepository, IActionsExecutioner actionsExecutioner)
        {
            _deviceRepository = deviceRepository;

            var status = _statusSubject.Publish();
            StatusMessageObservable = status.AsObservable();

            //actionsExecutioner.RegisterAction(VerificationTestStep.OnCorrectionTestStart, test =>
            //{
            //    if (!_mockedLiveReadValues.ContainsKey(itemNumber))
            //    {
            //        var value =
            //            await MessageInteractions.GetInputNumber.Handle(
            //                    $"Enter simulating value for {itemNumber.Description} - #{itemNumber.Number}:")
            //                .ObserveOn(RxApp.MainThreadScheduler);

            //        _mockedLiveReadValues.Add(itemNumber, value);
            //    }
            //});

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
}