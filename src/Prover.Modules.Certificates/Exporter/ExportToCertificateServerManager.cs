using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.Modules.Certificates.Storage;

namespace Prover.Modules.Certificates.Exporter
{
    public class ExportToCertificateServerManager : IExportTestRun
    {
        private readonly ProverContext _proverContext;       
        private readonly ICertificateStore<Instrument> _instrumentStore;

        public ExportToCertificateServerManager(ProverContext proverContext, ICertificateStore<Instrument> instrumentStore)
        {
            _proverContext = proverContext;
            _instrumentStore = instrumentStore;
        }

        public async Task<bool> Export(Instrument instrumentForExport)
        {
            if (CheckIfServerAvailable())
                try
                {
                    _proverContext.Entry(instrumentForExport).State = EntityState.Detached;
                    //instrumentForExport.ExportedDateTime = DateTime.Now;
                    await _instrumentStore.UpsertAsync(instrumentForExport);

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
