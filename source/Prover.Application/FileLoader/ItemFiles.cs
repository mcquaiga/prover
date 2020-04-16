using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Prover.Application.FileLoader
{
    public static class ItemFiles
    {
        public static Dictionary<string, string> MiniMaxItemFile 
            => _lazy.Value.Items;

        private static readonly Lazy<ItemAndTestFile> _lazy = new Lazy<ItemAndTestFile>(
            () => DeserializeItemFile(File.ReadAllText("SampleData\\MiniMax.json"))
        );

        public static Dictionary<string, string> PressureTest(int testNumber) => _lazy.Value.PressureTests.ElementAt(testNumber);
        public static Dictionary<string, string> Temperature(int testNumber) => _lazy.Value.TemperatureTests.ElementAt(testNumber);

        public static Dictionary<string, string> TempLowItems => Temperature(0);
        public static Dictionary<string, string> TempMidItems => Temperature(1);
        public static Dictionary<string, string> TempHighItems => Temperature(2);

        private static ItemAndTestFile DeserializeItemFile(string jsonString)
        {
            return JsonConvert.DeserializeObject<ItemAndTestFile>(jsonString);
        }

        private class ItemAndTestFile
        {
            public string DeviceId { get; set; }
            public Dictionary<string, string> Items { get; set; }

            public ICollection<Dictionary<string, string>> PressureTests { get; set; }
            public ICollection<Dictionary<string, string>> TemperatureTests { get; set; }
        }
        //internal class ItemFileConverter : JsonConverter
        //{
        //    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) => throw new NotImplementedException();

        //    public override bool CanConvert(Type objectType) => throw new NotImplementedException();
        //}

        //public class ItemAndTestFile
        //{
        //    //[JsonConstructor]
        //    //public ItemAndTestFile(
        //    //    Guid deviceId,
        //    //    Dictionary<string, string> items,
        //    //    IEnumerable<Dictionary<string, string>> pressureTests,
        //    //    IEnumerable<Dictionary<string, string>> temperatureTests)
        //    //{
        //    //    DeviceId = deviceId;
        //    //    Items = items;
        //    //    PressureTests = pressureTests.ToList();
        //    //    TemperatureTests = temperatureTests.ToList();
        //    //}

        //    public string DeviceId { get; set; }
        //    public Dictionary<string, string> Items { get; set; }

        //    public ICollection<Dictionary<string, string>> PressureTests { get; set; }
        //    public ICollection<Dictionary<string, string>> TemperatureTests { get; set; }
        //}
    }
}
