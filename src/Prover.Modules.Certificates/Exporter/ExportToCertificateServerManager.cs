using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.Modules.Certificates.Models;
using Prover.Modules.Certificates.Storage;

namespace Prover.Modules.Certificates.Exporter
{
    public class ExportToCertificateServerManager : IExportTestRun
    {    
        private readonly ICertificateStore<CertificateInstrument> _instrumentStore;

        public ExportToCertificateServerManager(ICertificateStore<CertificateInstrument> instrumentStore)
        {
            _instrumentStore = instrumentStore;
        }

        public async Task<bool> Export(Instrument instrumentForExport)
        {
            if (CheckIfServerAvailable())
                try
                {
                    var instrumentJson = JsonConvert.SerializeObject(instrumentForExport, Formatting.Indented, new JsonSerializerSettings(){ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
                    var newInstrument = JsonConvert.DeserializeObject<Instrument>(instrumentJson);
                    await _instrumentStore.UpsertAsync(newInstrument as CertificateInstrument);

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            return false;
        }

        private bool CheckIfServerAvailable()
        {
            return true;
        }

        public async Task<bool> Export(IEnumerable<Instrument> instrumentsForExport)
        {
            foreach (var instrument in instrumentsForExport)
            {
                await Export(instrument);
            }
            return true;
        }

        public bool CanExport(Instrument instrument)
        {
            return true;
        }

 
    }
}
