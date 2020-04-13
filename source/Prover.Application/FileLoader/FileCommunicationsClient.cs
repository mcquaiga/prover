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
using Prover.Application.Services.LiveReadCorrections;
using Prover.Application.Verifications;
using Prover.Application.Verifications.Events;
using Prover.Shared.Extensions;
using ReactiveUI;

namespace Prover.Application.FileLoader
{
    public class LiveReadSimulator
    {
        private FileDeviceClient _fileDeviceClient;

        private readonly Dictionary<ItemMetadata, decimal> _mockedLiveReadValues =
                new Dictionary<ItemMetadata, decimal>();

        private readonly List<ItemLiveReadStatus> _liveReadItems = new List<ItemLiveReadStatus>();

        public LiveReadSimulator(FileDeviceClient fileDeviceClient)
        {
            _fileDeviceClient = fileDeviceClient;
        }

        private async Task<IEnumerable<ItemValue>> GetItemsAfterLiveRead()
        {
            var testNumber = await MessageInteractions.GetInputInteger.Handle("Enter test number to load (1, 2, 3):");
            _mockedLiveReadValues.Clear();
            return _fileDeviceClient._itemFile.TemperatureTests[testNumber].ToList().Union(_fileDeviceClient._itemFile.PressureTests[testNumber]).ToList();
        }
    }

    public class FileDeviceClient : ICommunicationsClient
    {
        private readonly CompositeDisposable _cleanup;

        private readonly IDeviceRepository _deviceRepository;

        private readonly ISubject<StatusMessage> _statusSubject = new Subject<StatusMessage>();
        private ItemAndTestFile _itemFile;
        private readonly LiveReadSimulator _liveReadSimulator;

        public FileDeviceClient(ItemAndTestFile itemFile)
        {
            _liveReadSimulator = new LiveReadSimulator(this);
            _itemFile = itemFile;

            var status = _statusSubject.Publish();
            StatusMessageObservable = status.AsObservable();

            VerificationEvents.CorrectionTests.OnLiveReadStart.Subscribe(context =>
            {
                var random = new Random(DateTime.Now.Second);
                var liveReader = context.Input;

                _liveReadSimulator._liveReadItems.Clear();
                _liveReadSimulator._mockedLiveReadValues.Clear();

                liveReader.LiveReadItems
                          .Select(itemStatus => itemStatus.DeepClone())
                          .ForEach(_liveReadSimulator._liveReadItems.Add)
                          .ForEach(i => _liveReadSimulator._mockedLiveReadValues.Add(i.Item, i.TargetValue + GetRandom()));

                decimal GetRandom()
                {
                    return Round.Gauge(
                            random.Next(1, 100) / 100m);
                    
                }
            });

            _cleanup = new CompositeDisposable(status.Connect());
        }

        public FileDeviceClient(IDeviceRepository deviceRepository, string filePath)
        {
            _liveReadSimulator = new LiveReadSimulator(this);
            _deviceRepository = deviceRepository;
            FilePath = filePath;

            _itemFile = Observable.StartAsync(async () => await ItemLoader.LoadFromFile(_deviceRepository, filePath))
                                  .Wait();
        }

        public bool IsConnected { get; private set; }

        public IObservable<StatusMessage> StatusMessageObservable { get; }

        public string FilePath { get; private set; }

        public void Cancel()
        {
        }

        public async Task ConnectAsync(int retryAttempts = 10, TimeSpan? timeout = null)
        {
            //if (_itemFile == null)
            //{
            //    if (string.IsNullOrEmpty(FilePath))
            //        throw new NullReferenceException("FilePath cannot be null.");

            //    _itemFile = await ItemLoader.LoadFromFile(_deviceRepository, FilePath);
            //}

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
            if (_liveReadSimulator._mockedLiveReadValues.Any())
                return await _liveReadSimulator.GetItemsAfterLiveRead();

            if (itemNumbers == null)
                return _itemFile.Device.Values;

            return _itemFile.Device.Values.Where(i => itemNumbers.Contains(i.Metadata));
        }

        public async Task<ItemValue> LiveReadItemValue(ItemMetadata itemNumber)
        {
            await Task.CompletedTask;
            return ItemValue.Create(itemNumber, _liveReadSimulator._mockedLiveReadValues[itemNumber]);
        }

        public Task<bool> SetItemValue(int itemNumber, string value) => throw new NotImplementedException();
    }
}