using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using Newtonsoft.Json;
using Prover.Core.IO;

namespace Prover.Core.Settings
{
    public static class SettingsManager
    {
        private const string SettingsFileName = "settings.conf";
        private static readonly string SettingsPath = Path.Combine(Environment.CurrentDirectory, SettingsFileName);
        private static Settings _singletonInstance;

        public static Settings SettingsInstance
        {
            get
            {               
                return _singletonInstance;
            }
            set { _singletonInstance = value; }
        }

        public static async Task RefreshSettings()
        {
            _singletonInstance = await LoadSettings();
            await Save();
        }

        private static async Task<Settings> LoadSettings()
        {
            return await Task.Run(() =>
            {
                var fileSystem = new FileSystem();
                var settings = (fileSystem.FileExists(SettingsPath) ? new SettingsReader(fileSystem).Read(SettingsPath) : null) ?? new Settings();
                settings.SetDefaults();
                return settings;
            });
        }

        public static async Task Save()
        {           
            await Task.Run(() =>
            {
                var fileSystem = new FileSystem();

                if (!fileSystem.DirectoryExists(Path.GetDirectoryName(SettingsPath)))
                    fileSystem.CreateDirectory(Path.GetDirectoryName(SettingsPath));
                new SettingsWriter(fileSystem).Write(SettingsPath, _singletonInstance);
            });
        }
    }

    public class SettingsReader
    {
        private readonly IFileSystem _fileSystem;

        public SettingsReader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public Settings Read(string path)
        {
            var jsonText = _fileSystem.ReadAllText(path);
            if (string.IsNullOrEmpty(jsonText))
                return new Settings();

            return JsonConvert.DeserializeObject<Settings>(jsonText);
        }
    }

    public class SettingsWriter
    {
        private readonly IFileSystem _fileSystem;

        public SettingsWriter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Write(string path, Settings config)
        {
            var jsonText = JsonConvert.SerializeObject(config, Formatting.Indented);
            _fileSystem.WriteAllText(path, jsonText);
        }
    }
}