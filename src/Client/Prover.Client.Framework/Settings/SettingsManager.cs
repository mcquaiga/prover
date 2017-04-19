using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prover.Client.Framework.IO;

namespace Prover.Client.Framework.Settings
{
    public static class SettingsManager
    {
        private const string SettingsFileName = "settings.conf";
        private static readonly string SettingsPath = Path.Combine(Environment.CurrentDirectory, SettingsFileName);

        public static ProverSettings SettingsInstance { get; set; }

        public static async Task RefreshSettings()
        {
            SettingsInstance = await LoadSettings();
            await Save();
        }

        private static async Task<ProverSettings> LoadSettings()
        {
            return await Task.Run(() =>
            {
                var fileSystem = new FileSystem();
                var settings = (fileSystem.FileExists(SettingsPath)
                                   ? new SettingsReader(fileSystem).Read(SettingsPath)
                                   : null) ?? new ProverSettings();
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
                new SettingsWriter(fileSystem).Write(SettingsPath, SettingsInstance);
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

        public ProverSettings Read(string path)
        {
            var jsonText = _fileSystem.ReadAllText(path);
            if (string.IsNullOrEmpty(jsonText))
                return new ProverSettings();

            return JsonConvert.DeserializeObject<ProverSettings>(jsonText);
        }
    }

    public class SettingsWriter
    {
        private readonly IFileSystem _fileSystem;

        public SettingsWriter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Write(string path, ProverSettings config)
        {
            var jsonText = JsonConvert.SerializeObject(config, Formatting.Indented);
            _fileSystem.WriteAllText(path, jsonText);
        }
    }
}