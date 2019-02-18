using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prover.Core.IO;

namespace Prover.Core.Settings
{

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

            var uniencoding = new UnicodeEncoding();
            byte[] result = uniencoding.GetBytes(jsonText);
            
            using (FileStream sourceStream = File.Open(path, FileMode.Create))
            {
                sourceStream.Seek(0, SeekOrigin.Begin);
                await sourceStream.WriteAsync(result, 0, result.Length);
            }

            _fileSystem.WriteAllText(path, jsonText);
        }
    }
}