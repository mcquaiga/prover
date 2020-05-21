using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Prover.Modules.DevTools.SampleData
{
    public static class DeviceSamples
    {
        //public static DeviceInstance MiniMax => SampleItemFiles.MiniMaxDevice;
        //public static DeviceInstance MiniAt => SampleItemFiles.MiniAtDevice;
        //public static DeviceInstance Adem => SampleItemFiles.AdemDevice;

        //public static List<DeviceInstance> All = new List<DeviceInstance>() { MiniMax, MiniAt, Adem };

    }

    public static class VerificationSamples
    {

    }


    public static class SampleItemFiles
    {
        static SampleItemFiles()
        {
            //MiniAtDevice = JsonConvert.DeserializeObject<DeviceInstance>(File.ReadAllText(".\\SampleData\\MiniAt.json"));
            //AdemDevice = JsonConvert.DeserializeObject<DeviceInstance>(File.ReadAllText(".\\SampleData\\Adem.json"));
            //MiniMaxDevice = JsonConvert.DeserializeObject<DeviceInstance>(File.ReadAllText(".\\SampleData\\MiniMax.json"));
        }

        //public static DeviceInstance MiniMaxDevice { get; }
        //public static DeviceInstance AdemDevice { get; }
        //public static DeviceInstance MiniAtDevice { get; }


        public static Dictionary<string, string> MiniMaxItemFile => _lazy.Value.Items;
        private static readonly Lazy<ItemAndTestFile> _lazy = new Lazy<ItemAndTestFile>(() => DeserializeItemFile(File.ReadAllText("SampleData\\MiniMax.json")));

        public static Dictionary<string, string> PressureTest(int testNumber) => _lazy.Value.PressureTests.ElementAt(testNumber);


        public static Dictionary<string, string> TempLowItems => _lazy.Value.TemperatureTests.ElementAt(0);
        public static Dictionary<string, string> TempMidItems => _lazy.Value.TemperatureTests.ElementAt(1);
        public static Dictionary<string, string> TempHighItems => _lazy.Value.TemperatureTests.ElementAt(2);

        public static ItemAndTestFile DeserializeItemFile(string jsonString)
        {
            return JsonConvert.DeserializeObject<ItemAndTestFile>(jsonString);
        }

        public static ItemAndTestFile LoadFromFile(string filePath)
        {
            return DeserializeItemFile(File.ReadAllText(filePath));
        }

        public class ItemAndTestFile
        {
            [JsonConstructor]
            public ItemAndTestFile(Dictionary<string, string> items,
                IEnumerable<Dictionary<string, string>> pressureTests,
                IEnumerable<Dictionary<string, string>> temperatureTests)
            {
                Items = items;
                PressureTests = pressureTests.ToList();
                TemperatureTests = temperatureTests.ToList();
            }

            public Dictionary<string, string> Items { get; set; }

            public ICollection<Dictionary<string, string>> PressureTests { get; set; }
            public ICollection<Dictionary<string, string>> TemperatureTests { get; set; }
        }
    }
}
