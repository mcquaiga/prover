using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.Storage;
using Prover.Core.Models.Instruments;
using Prover.Core.Shared.Data;

namespace Prover.Core.Services
{
    public class TestRunService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IProverStore<Instrument> _instrumentStore;

        public TestRunService(IDbContextScopeFactory dbContextScopeFactory, IProverStore<Instrument> instrumentStore)
        {
            _dbContextScopeFactory = dbContextScopeFactory;
            _instrumentStore = instrumentStore;
        }

        public async Task<List<Instrument>> GetAllUnexported()
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _instrumentStore.Query(i => i.ExportedDateTime == null)
                    .ToListAsync();
            }
        }

        public async Task ArchiveTest(Instrument instrument)
        {
            instrument.ArchivedDateTime = DateTime.UtcNow;
            await Save(instrument);
        }

        public async Task Save(Instrument instrument)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                await _instrumentStore.Upsert(instrument);
                await dbContextScope.SaveChangesAsync();
            }
        }
    }
}