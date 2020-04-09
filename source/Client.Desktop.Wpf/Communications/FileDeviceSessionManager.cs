using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using Prover.Application.FileLoader;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;

namespace Client.Desktop.Wpf.Communications
{
    //public class FileDeviceSessionManager : IDeviceSessionManager
    //{
    //    private readonly IDeviceRepository _deviceRepository;
    //    private ItemAndTestFile _itemFile;

    //    public FileDeviceSessionManager(IDeviceRepository deviceRepository) => _deviceRepository = deviceRepository;

    //    public DeviceInstance Device { get; private set; }
    //    public bool SessionInProgress { get; private set; }

    //    public string FilePath { get; private set; }

    //    public async Task Connect()
    //    {
    //        SessionInProgress = true;
    //        await Task.CompletedTask;
    //    }

    //    public Task Disconnect() => throw new NotImplementedException();

    //    public async Task<ICollection<ItemValue>> DownloadCorrectionItems()
    //    {
    //        var testNumber = await MessageInteractions.GetInputInteger.Handle("Enter test number to load (1, 2, 3):");

    //        return _itemFile.TemperatureTests[testNumber].ToList()
    //            .Union(_itemFile.PressureTests[testNumber]).ToList();
    //    }

    //    public Task<ICollection<ItemValue>> DownloadCorrectionItems(ICollection<ItemMetadata> items) =>
    //        throw new NotImplementedException();

    //    public Task EndSession() => throw new NotImplementedException();

    //    public ICollection<ItemMetadata> GetItemsToDownload() => throw new NotImplementedException();

    //    public Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<ItemMetadata> itemsToDownload = null) =>
    //        throw new NotImplementedException();

    //    public Task<ItemValue> LiveReadItemValue(ItemMetadata item) => throw new NotImplementedException();

    //    public void SetFilePath(string filePath)
    //    {
    //        if (File.Exists(filePath))
    //            FilePath = filePath;
    //    }

    //    public void SetItemFileAndTestDefinition(ItemAndTestFile testFile)
    //    {
    //        _itemFile = testFile;
    //    }

    //    public async Task<DeviceInstance> StartSession(DeviceType deviceType)
    //    {
    //        SessionInProgress = true;
    //        if (string.IsNullOrEmpty(FilePath))
    //            throw new NullReferenceException("ItemFile cannot be null. Call SetItemFileAndTestDefinition first.");

    //        _itemFile = await ItemLoader.LoadFromFile(_deviceRepository, FilePath);
    //        Device = _itemFile.Device;

    //        return Device;
    //    }

    //    public Task<ItemValue> WriteItemValue(ItemMetadata item, string value) => throw new NotImplementedException();
    //}
}