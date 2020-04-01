using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Prover.Application.Interfaces;
using Prover.Domain.EvcVerifications;

namespace Prover.Application.Services
{
    public class ExportManager : IExportVerificationTest
    {
        public async Task<bool> Export(EvcVerificationTest instrumentForExport) => true;

        public async Task<bool> Export(IEnumerable<EvcVerificationTest> instrumentsForExport) => true;

    }
}
