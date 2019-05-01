using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Repository.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Devices.Honeywell.Core.Repository
{
    public class FileStreamReader : IStreamReader
    {
        #region Fields

        private const string ItemDefinitionFileName = "Items";

        private const string TypeFileName = "Type_";

        private static readonly string ItemDefinitionsFolder = $@"{Environment.CurrentDirectory}\DeviceTypes";

        #endregion

        #region Methods

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

        private static IEnumerable<string> FindDeviceFiles()
        {
            return Directory.GetFiles(ItemDefinitionsFolder, $"{TypeFileName}*.json");
        }

        #endregion
    }
}