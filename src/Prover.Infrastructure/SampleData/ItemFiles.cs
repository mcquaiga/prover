using System;
using System.Collections.Generic;
using System.IO;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Prover.Infrastructure.SampleData
{
    public static class ItemFiles
    {
        public static Dictionary<string, string> MiniMaxItemFile 
            => _lazy.Value;

        private static readonly Lazy<Dictionary<string, string>> _lazy = new Lazy<Dictionary<string, string>>(
            () => DeserializeItemFile(File.ReadAllText("SampleData\\MiniMax.json"))
        );

        public static Dictionary<string, string> PressureHighItems 
            => DeserializeItemFile("{\"8\":\"  80.134\",\"44\":\"  6.4402\",\"47\":\"  1.0076\"}");

        public static Dictionary<string, string> TempLowItems
            => DeserializeItemFile("{\"26\":\" 32.44\",\"34\":\"  60.00\",\"35\":\"  0.3490\",\"45\":\"  1.0560\"}");
        public static Dictionary<string, string> TempMidItems
            => DeserializeItemFile("{\"26\":\"   59.65\",\"35\":\"  0.1662\",\"45\":\"  1.0007\"}");

        public static Dictionary<string, string> TempHighItems
            => DeserializeItemFile("{\"26\":\"   89.47\",\"35\":\" -0.4902\",\"45\":\"  0.9463\"}");
        
        private static Dictionary<string, string> DeserializeItemFile(string jsonString)
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
        }
    }
}
