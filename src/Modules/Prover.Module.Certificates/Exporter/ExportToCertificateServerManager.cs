using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prover.Core.DTOs;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.Modules.Certificates.Models;
using Prover.Modules.Certificates.Storage;

namespace Prover.Modules.Certificates.Exporter
{
    public class ExportToCertificateServerManager : IExportTestRun
    {    
        public ExportToCertificateServerManager()
        {
        }

        public async Task<bool> Export(Instrument instrumentForExport)
        {
            var apiUri = "http://localhost:58508/api/instruments";

            if (CheckIfServerAvailable())
                try
                {
                    using (var webClient = new HttpClient())
                    {
                        var dto = instrumentForExport.ToDto();
                        var instrumentJson = JsonConvert.SerializeObject(instrumentForExport, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                        await webClient.PostAsync(apiUri, new StringContent(instrumentJson, Encoding.UTF8, "application/json"));
                    }                  

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
