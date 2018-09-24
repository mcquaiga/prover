using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.Core.Models.Instruments;
using Prover.Core.Shared.Data;

namespace Prover.Core.Services
{
    public class TestRunService
    {
        private readonly IProverStore<Instrument> _instrumentStore;

        public TestRunService(IProverStore<Instrument> instrumentStore)
        {
            _instrumentStore = instrumentStore;
        }

        public IEnumerable<Instrument> GetAllUnexported()
        {
            return _instrumentStore
                .Query(i => i.ExportedDateTime == null);
        }

        public IEnumerable<Instrument> GetTestRunByCertificate(Guid certificateId)
        {
            return _instrumentStore.Query(i => i.CertificateId == certificateId);
        }

        public async Task ArchiveTest(Instrument instrument)
        {
            instrument.ArchivedDateTime = DateTime.UtcNow;
            await _instrumentStore.Upsert(instrument);
        }

        public async Task Save(Instrument instrument)
        {
            foreach (var vt in instrument.VerificationTests)
            {
                   
            }
            await _instrumentStore.Upsert(instrument);
        }
    }
}
