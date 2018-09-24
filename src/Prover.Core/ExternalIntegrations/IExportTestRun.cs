using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.Core.Models.Instruments;

namespace Prover.Core.ExternalIntegrations
{
    public interface IExportTestRun
    {
        Task<bool> Export(Instrument instrumentForExport);
        Task<bool> Export(IEnumerable<Instrument> instrumentsForExport);
        Task<bool> ExportFailedTest(string companyNumber);
    }
}