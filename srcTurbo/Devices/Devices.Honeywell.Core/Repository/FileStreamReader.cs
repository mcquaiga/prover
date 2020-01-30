using Devices.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Devices.Core.Repository;

namespace Devices.Honeywell.Core.Repository
{
    public class FileStreamReader : IStreamReader
    {
        public FileStreamReader(string devicesRootDirectory = null)
        {
            ItemDefinitionsFolder = $@"{devicesRootDirectory}/DeviceTypes";

            if (string.IsNullOrEmpty(devicesRootDirectory))
                ItemDefinitionsFolder = $@"{Environment.CurrentDirectory}/DeviceTypes";

            if (!Directory.Exists(ItemDefinitionsFolder))
                throw new DirectoryNotFoundException($"Directory path {ItemDefinitionsFolder} does not exist.");
        }

        public IEnumerable<StreamReader> GetDeviceTypeReaders()
        {
            return FindDeviceFiles()
                .Select(df => new StreamReader(df));
        }

        public StreamReader GetItemDefinitionsReader(string name)
        {
            var path = $@"{ItemDefinitionsFolder}/{name}.json";
            return new StreamReader(path);
        }

        public StreamReader GetItemsReader()
        {
            var path = $@"{ItemDefinitionsFolder}/{ItemDefinitionFileName}.json";
            return new StreamReader(path);
        }

        private const string ItemDefinitionFileName = "Items";

        private const string TypeFileName = "Type_";

        private readonly string ItemDefinitionsFolder;

        private IEnumerable<string> FindDeviceFiles()
        {
            return Directory.GetFiles(ItemDefinitionsFolder, $"{TypeFileName}*.json");
        }
    }
}