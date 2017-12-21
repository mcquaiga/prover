using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prover.Core.IO;
using Prover.Core.Shared.Data;
using Prover.Core.Shared.Domain;
using Prover.Core.Storage;

namespace Prover.Core.Settings
{
    public interface ISettingsService
    {
        LocalSettings LocalSettingsInstance { get; }
        SharedSettings SharedSettingsInstance { get; }
        void RefreshSettings();

        Task SaveLocalSettingsAsync();
        void SaveLocalSettings();
        Task SaveSharedSettings();
    }

    public class SettingsService : ISettingsService
    {
        private const string SettingsFileName = "settings.conf";
        private static readonly string AppDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EvcProver");
        private static readonly string SettingsPath = Path.Combine(AppDirectory, SettingsFileName);
        private readonly KeyValueStore _keyValueStore;

        public SettingsService(KeyValueStore keyValueStore)
        {
            _keyValueStore = keyValueStore;
            RefreshSettings();
        }

        public LocalSettings LocalSettingsInstance { get; private set; }
        public SharedSettings SharedSettingsInstance { get; private set; }

        public void RefreshSettings()
        {
            LocalSettingsInstance = LoadLocalSettings();

            if (_keyValueStore != null)
                SharedSettingsInstance = LoadSharedSettings(_keyValueStore);
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

            new SettingsWriter(fileSystem).Write(SettingsPath, LocalSettingsInstance);
        }

        public async Task SaveSharedSettings()
        {
            var kv = new KeyValue() { Id = "SharedSettings", Value = JsonConvert.SerializeObject(SharedSettingsInstance) };
            await _keyValueStore.Upsert(kv);
        }

        private static SharedSettings LoadSharedSettings(KeyValueStore keyValueStore)
        {
            var settingsKeyValue = keyValueStore.Query(k => k.Id == "SharedSettings").FirstOrDefault();

            if (string.IsNullOrEmpty(settingsKeyValue?.Value))
                return SharedSettings.Create();

            return JsonConvert.DeserializeObject<SharedSettings>(settingsKeyValue.Value);
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

    //public static class SettingsManager
    //{    
    //    private const string SettingsFileName = "settings.conf";
    //    private static readonly string AppDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EvcProver");
    //    private static readonly string SettingsPath = Path.Combine(AppDirectory, SettingsFileName);
    //    private static KeyValueStore _keyValueStore;

    //    public static LocalSettings LocalSettingsInstance { get; set; }
    //    public static SharedSettings SharedSettingsInstance { get; set; }

    //    public static void Initialize(KeyValueStore keyValueStore)
    //    {
    //        _keyValueStore = keyValueStore;
    //        Task.Run(async () => await RefreshSettings());
    //    }

    //    public static async Task RefreshSettings()
    //    {
    //        LocalSettingsInstance = await LoadLocalSettings();

    //        if (_keyValueStore != null)
    //            SharedSettingsInstance = await LoadSharedSettings(_keyValueStore);
    //    }

    //    private static async Task<LocalSettings> LoadLocalSettings()
    //    {
    //        return await Task.Run(() =>
    //        {
    //            var fileSystem = new FileSystem();
    //            Directory.CreateDirectory(AppDirectory);
    //            var settings = (fileSystem.FileExists(SettingsPath)
    //                               ? new SettingsReader(fileSystem).Read(SettingsPath)
    //                               : null) ?? new LocalSettings();
    //            return settings;
    //        });
    //    }

    //    private static async Task<SharedSettings> LoadSharedSettings(KeyValueStore keyValueStore)
    //    {
    //        var settingsKeyValue = await keyValueStore.Query(k => k.Id == "SharedSettings").FirstOrDefaultAsync();

    //        if (string.IsNullOrEmpty(settingsKeyValue?.Value))
    //            return SharedSettings.Create();

    //        return JsonConvert.DeserializeObject<SharedSettings>(settingsKeyValue.Value);
    //    }

    //    public static async Task SaveLocalSettingsAsync()
    //    {
    //        await Task.Run(() => SaveLocalSettings());
    //    }

    //    public static void SaveLocalSettings()
    //    {           
    //        var fileSystem = new FileSystem();

    //        if (!fileSystem.DirectoryExists(Path.GetDirectoryName(AppDirectory)))
    //            fileSystem.CreateDirectory(Path.GetDirectoryName(AppDirectory));

    //        new SettingsWriter(fileSystem).Write(SettingsPath, LocalSettingsInstance);
    //    }

    //    public static async Task SaveSharedSettings()
    //    {
    //        var kv = new KeyValue() { Id = "SharedSettings", Value = JsonConvert.SerializeObject(SharedSettingsInstance)};
    //        await _keyValueStore.Upsert(kv);
    //    }
    //}

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