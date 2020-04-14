using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using Newtonsoft.Json;
using Prover.Application.Interactions;

namespace Prover.Application.FileLoader
{
    public class ItemLoader
    {
        public static async Task<string> GetFileInput() => await MessageInteractions.OpenFileDialog.Handle("Open file");

        public static async Task<ItemAndTestFile> LoadFromFile(IDeviceRepository devices, string filePath)
        {
            if (!File.Exists(filePath)) return null;

            var json = File.ReadAllText(filePath);
            var itemFile = JsonConvert.DeserializeObject<JsonItemAndTestFile>(json);

            var deviceType = devices.GetById(Guid.Parse(itemFile.DeviceId));
            if (deviceType == null)
                throw new NullReferenceException(nameof(deviceType));

            //var itemValues = deviceType.ToItemValues(itemFile.Items);
            var deviceInstance = deviceType.CreateInstance(itemFile.Items);

            var testNum = -1;
            var pressures = itemFile.PressureTests
                .Select(p => deviceType.ToItemValues(p))
                .ToDictionary(x =>
                {
                    testNum++;
                    return testNum;
                }, values => values.ToList());
            testNum = -1;
            var temps = itemFile.TemperatureTests
                .Select(p => deviceType.ToItemValues(p))
                .ToDictionary(x =>
                {
                    testNum++;
                    return testNum;
                }, values => values.ToList());

            return new ItemAndTestFile
            {
                Device = deviceInstance,
                PressureTests = pressures,
                TemperatureTests = temps
            };
        }

        #region Nested type: ItemAndTestFile

       
        #endregion

        #region Nested type: JsonItemAndTestFile

        private class JsonItemAndTestFile
        {
            public string DeviceId { get; set; }
            public Dictionary<string, string> Items { get; set; }

            public ICollection<Dictionary<string, string>> PressureTests { get; set; }
            public ICollection<Dictionary<string, string>> TemperatureTests { get; set; }
            public Dictionary<string, string> VolumeTest { get; set; }
        }

        #endregion
    }

    public class ItemAndTestFile
    {
        public DeviceInstance Device { get; set; }

        public Dictionary<int, List<ItemValue>> PressureTests { get; set; }
        public Dictionary<int, List<ItemValue>> TemperatureTests { get; set; }
        public Tuple<List<ItemValue>, List<ItemValue>> VolumeTest { get; set; }
    }

}