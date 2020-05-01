using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Devices.Communications.Interfaces;
using Devices.Communications.Status;
using Devices.Core.Items;
using Devices.Core.Repository;
using Prover.Application.Services.LiveReadCorrections;
using Prover.Application.Verifications;
using Prover.Shared.Extensions;

namespace Prover.Modules.DevTools.FileLoader
{
    public class LiveReadSimulator
    {
        private readonly List<ItemLiveReadStatus> _liveReadItems = new List<ItemLiveReadStatus>();

        private readonly Dictionary<ItemMetadata, decimal> _mockedLiveReadValues =
                new Dictionary<ItemMetadata, decimal>();

        public LiveReadSimulator()
        {
            VerificationEvents.CorrectionTests.OnLiveReadStart.Subscribe(context =>
            {
                var random = new Random(DateTime.Now.Second);
                var liveReader = context.Input;

                _liveReadItems.Clear();
                _mockedLiveReadValues.Clear();

                liveReader.LiveReadItems
                          .Select(itemStatus => (ItemLiveReadStatus)itemStatus.Clone())
                          .ForEach(_liveReadItems.Add)
                          .ForEach(i => _mockedLiveReadValues.Add(i.Item, i.TargetValue + GetRandom()));

                decimal GetRandom() => Round.Gauge(
                        random.Next(1, 100) / 100m);
            });

            VerificationEvents.CorrectionTests.OnLiveReadComplete.Subscribe(testPoint => { });
        }

        public ItemValue GenerateLiveReadValue(ItemMetadata item) => ItemValue.Create(item, _mockedLiveReadValues[item]);

        //private async Task<IEnumerable<ItemValue>> GetItemsAfterLiveRead()
        //{
        //    var testNumber = await MessageInteractions.GetInputInteger.Handle("Enter test number to load (1, 2, 3):");
        //    _mockedLiveReadValues.Clear();
        //    return _fileDeviceClient._itemFile.TemperatureTests[testNumber].ToList().Union(_fileDeviceClient._itemFile.PressureTests[testNumber]).ToList();
        //}
    }

    public class FileDeviceClient : ICommunicationsClient
    {
        private readonly CompositeDisposable _cleanup;
        private readonly ItemAndTestFile _itemFile;
        private readonly LiveReadSimulator _liveReadSimulator;
        private readonly ISubject<StatusMessage> _statusSubject = new Subject<StatusMessage>();

        private readonly List<ItemValue> _queuedValues = new List<ItemValue>();

        private FileDeviceClient()
        {
            _liveReadSimulator = new LiveReadSimulator();
            var status = _statusSubject.Publish();
            StatusMessageObservable = status.AsObservable();
            _cleanup = new CompositeDisposable(status.Connect());

            VerificationEvents.CorrectionTests.BeforeDownload.Subscribe(context =>
            {
                _queuedValues.Clear();
                _queuedValues.AddRange(Enumerable.Union<ItemValue>(_itemFile.PressureTests[context.Input.TestNumber], _itemFile.TemperatureTests[context.Input.TestNumber]));
            });
        }

        public FileDeviceClient(ItemAndTestFile itemFile) : this() => _itemFile = itemFile;
        public bool IsConnected { get; private set; }
        public IObservable<StatusMessage> StatusMessageObservable { get; }

        public void Cancel()
        {
        }

        public async Task ConnectAsync(int retryAttempts = 10, TimeSpan? timeout = null)
        {
            IsConnected = true;
            await Task.CompletedTask;
        }

        public static FileDeviceClient Create(IDeviceRepository deviceRepository, string filePath)
        {
            var itemFile = Observable.StartAsync<ItemAndTestFile>(async () => await ItemLoader.LoadFromFile(deviceRepository, filePath))
                                     .Wait();
            return new FileDeviceClient(itemFile);
        }

        public static async Task<FileDeviceClient> CreateAsync(IDeviceRepository deviceRepository, string filePath)
        {
            var itemFile = await Observable.StartAsync<ItemAndTestFile>(async () => await ItemLoader.LoadFromFile(deviceRepository, filePath)).FirstAsync();
            return new FileDeviceClient(itemFile);
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
            await Task.CompletedTask;

            if (Enumerable.Any<ItemValue>(_queuedValues))
            {
                var values = Enumerable.Where<ItemValue>(_queuedValues, qv => Enumerable.Contains<ItemMetadata>(itemNumbers, qv.Metadata)).ToList();
                _queuedValues.Clear();
                return values;
            }

            if (itemNumbers == null)
                return _itemFile.Device.Values;

            return _itemFile.Device.Values.Where(i => Enumerable.Contains<ItemMetadata>(itemNumbers, i.Metadata));
        }

        public async Task<ItemValue> LiveReadItemValue(ItemMetadata itemNumber)
        {
            await Task.CompletedTask;

            return _liveReadSimulator.GenerateLiveReadValue(itemNumber);
        }

        public async Task<bool> SetItemValue(int itemNumber, string value)
        {
            var newValue = ItemValue.Create(_itemFile.Device.Values.GetItem(itemNumber).Metadata, value);
            _itemFile.Device.SetItemValues(new[] { newValue });
            return await Task.FromResult(true);
        }
    }
}