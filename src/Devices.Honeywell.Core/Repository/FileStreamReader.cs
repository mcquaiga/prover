using Devices.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace Devices.Honeywell.Core.Repository
{
    public class FileStreamReader : IStreamReader
    {
        private const string ItemDefinitionFileName = "Items";

        private const string TypeFileName = "Type_";

        private static readonly string ItemDefinitionsFolder = $@"{Environment.CurrentDirectory}\DeviceTypes";

        private static IEnumerable<string> FindDeviceFiles()
        {
            return Directory.GetFiles(ItemDefinitionsFolder, $"{TypeFileName}*.json");
        }

        public IEnumerable<StreamReader> GetDeviceTypeReaders()
        {
            return FindDeviceFiles()
                .Select(df => new StreamReader(df));
        }

        public StreamReader GetItemDefinitionsReader(string name)
        {
            var path = $@"{ItemDefinitionsFolder}\{name}.json";
            return new StreamReader(path);
        }

        public StreamReader GetItemsReader()
        {
            var path = $@"{ItemDefinitionsFolder}\{ItemDefinitionFileName}.json";
            return new StreamReader(path);
        }
    }
}