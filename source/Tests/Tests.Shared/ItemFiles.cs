using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Devices.Core.Items.ItemGroups;
using Prover.Shared;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Tests.Shared
{
    public static class ItemFiles
    {
        public static Dictionary<int, string> MiniMaxItemFile 
            => _lazy.Value;

        private static readonly Lazy<Dictionary<int, string>> _lazy = new Lazy<Dictionary<int, string>>(
            () => DeserializeItemFile(File.ReadAllText("Test-MiniMax.json"))
        );

        public static Dictionary<int, string> PressureHighItems 
            => DeserializeItemFile("{\"8\":\"  80.134\",\"44\":\"  6.4402\",\"47\":\"  1.0076\"}");

        public static Dictionary<int, string> TempLowItems
            => DeserializeItemFile("{\"26\":\" 32.44\",\"35\":\"  0.3490\",\"45\":\"  1.0560\"}");
        public static Dictionary<int, string> TempMidItems
            => DeserializeItemFile("{\"26\":\"   59.65\",\"35\":\"  0.1662\",\"45\":\"  1.0007\"}");
        public static Dictionary<int, string> TempHighItems
            => DeserializeItemFile("{\"26\":\"   89.47\",\"35\":\" -0.4902\",\"45\":\"  0.9463\"}");

        public static TemperatureItems HighTemperatureItems = new TemperatureItems()
        {
                Base = 60m,
                Factor = 0.9463m,
                GasTemperature = 89.47m,
                Units = TemperatureUnitType.F
        };

        private static Dictionary<int, string> ConvertToItemValues(Dictionary<string, string> dict)
        {
            return dict.ToDictionary(k => int.Parse((string) k.Key), v => v.Value);
        }

        private static Dictionary<int, string> DeserializeItemFile(string jsonString)
        {
            return ConvertToItemValues(JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString));
        }

        
    }
}
