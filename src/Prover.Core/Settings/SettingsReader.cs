using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prover.Core.IO;

namespace Prover.Core.Settings
{

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
                if (!_fileSystem.FileExists(path)) return new LocalSettings();

                using (FileStream SourceStream = File.Open(path, FileMode.Open))
                {
                    var result = new byte[SourceStream.Length];
                    await SourceStream.ReadAsync(result, 0, (int)SourceStream.Length);

                    var jsonText = System.Text.Encoding.UTF8.GetString(result);

                    if (string.IsNullOrEmpty(jsonText))
                        return new LocalSettings();

                    return JsonConvert.DeserializeObject<LocalSettings>(jsonText);
                }      
            }
            catch(Exception ex)
            {
                return new LocalSettings();
            }
                 
        }
    }
}