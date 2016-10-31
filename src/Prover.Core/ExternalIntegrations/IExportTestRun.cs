using Prover.Core.Models.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.ExternalIntegrations
{
    public interface IExportTestRun
    {
        Task<bool> Export(Instrument instrumentForExport);
        Task<bool> Export(IEnumerable<Instrument> instrumentsForExport);
    }
}
