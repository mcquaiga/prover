using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prover.Core.IO;
using Prover.Core.Shared.Domain;
using Prover.Core.Storage;

namespace Prover.Core.Settings
{
    public interface ISettingsService
    {
        LocalSettings Local { get; }
        SharedSettings Shared { get; }
        void RefreshSettings();

        Task SaveLocalSettingsAsync();
        void SaveLocalSettings();
        Task SaveSharedSettings();
    }

    public class SettingsService : ISettingsService
    {
        private const string SettingsFileName = "settings.conf";
        private const string SettingsKey = "SharedSettings";

        private static readonly string AppDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EvcProver");
        private static readonly string SettingsPath = Path.Combine(AppDirectory, SettingsFileName);

        private readonly KeyValueStore _keyValueStore;        

        public SettingsService(KeyValueStore keyValueStore)
        {
            _keyValueStore = keyValueStore;
            RefreshSettings();
        }

        public LocalSettings Local { get; private set; }
        public SharedSettings Shared { get; private set; }

        public void RefreshSettings()
        {
            Local = LoadLocalSettings();

            if (_keyValueStore != null)
                Shared = LoadSharedSettings(_keyValueStore);
        }

        public async Task SaveLocalSettingsAsync()
        {
            await Task.Run(() => SaveLocalSettings());
        }

        public void SaveLocalSettings()
        {
            var fileSystem = new FileSystem();

            if (!fileSystem.DirectoryExists(Path.GetDirectoryName(AppDirectory)))
                fileSystem.CreateDirectory(Path.GetDirectoryName(AppDirectory));

            new SettingsWriter(fileSystem).Write(SettingsPath, Local);
        }

        public async Task SaveSharedSettings()
        {
            var kv = new KeyValue() { Id = SettingsKey, Value = JsonConvert.SerializeObject(Shared) };
            await _keyValueStore.Upsert(kv);
        }

        private static SharedSettings LoadSharedSettings(KeyValueStore keyValueStore)
        {
            var settings = keyValueStore.GetValue<SharedSettings>(SettingsKey) ?? SharedSettings.Create();

            return settings;
        }

        private static LocalSettings LoadLocalSettings()
        {            
            var fileSystem = new FileSystem();
            Directory.CreateDirectory(AppDirectory);
            var settings = (fileSystem.FileExists(SettingsPath)
                                ? new SettingsReader(fileSystem).Read(SettingsPath)
                                : null) ?? new LocalSettings();
            return settings;
        }
    }

    public class SettingsReader
    {
        private readonly IFileSystem _fileSystem;

        public SettingsReader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public LocalSettings Read(string path)
        {
            var jsonText = _fileSystem.ReadAllText(path);
            if (string.IsNullOrEmpty(jsonText))
                return new LocalSettings();

            return JsonConvert.DeserializeObject<LocalSettings>(jsonText);
        }
    }

    public class SettingsWriter
    {
        private readonly IFileSystem _fileSystem;

        public SettingsWriter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Write(string path, LocalSettings config)
        {
            var jsonText = JsonConvert.SerializeObject(config, Formatting.Indented);
            _fileSystem.WriteAllText(path, jsonText);
        }
    }
}