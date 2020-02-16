using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Interfaces;

namespace Application.Settings
{

    public class JsonFileSettingsRepository
    {

    }


    public class SettingsWriter
    {
        private readonly IFileSystem _fileSystem;

        public SettingsWriter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public async Task Write(string path, LocalSettings config)
        {            
            var jsonText = JsonConvert.SerializeObject(config, Formatting.Indented);

            byte[] result = Encoding.UTF8.GetBytes(jsonText);
            
            using (FileStream sourceStream = File.Open(path, FileMode.Create))
            {
                sourceStream.Seek(0, SeekOrigin.Begin);
                await sourceStream.WriteAsync(result, 0, result.Length);
            }

            _fileSystem.WriteAllText(path, jsonText);
        }
    }

    public class SettingsReader
    {
        private readonly IFileSystem _fileSystem;

        public SettingsReader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public async Task<LocalSettings> Read(string path)
        {
            try
            {
                if (!_fileSystem.FileExists(path))
                    return null;
                
                using (FileStream SourceStream = File.Open(path, FileMode.Open))
                {
                    var result = new byte[SourceStream.Length];
                    await SourceStream.ReadAsync(result, 0, (int)SourceStream.Length);

                    var jsonText = System.Text.Encoding.UTF8.GetString(result);

                    if (string.IsNullOrEmpty(jsonText))
                        return null;

                    return JsonConvert.DeserializeObject<LocalSettings>(jsonText);
                }      
            }
            catch (Exception)
            {
                return null;
            }
                 
        }
    }
}