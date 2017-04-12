using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Domain.Instrument;
using Prover.Domain.Models.Instrument;
using Prover.Domain.Models.Prover;
using Prover.Domain.Verification.TestPoints;
using Prover.Domain.Verification.TestRun;
using Prover.Shared.Enums;
using Prover.Shared.Storage;

namespace Prover.Services.ServiceClients
{
    public interface IProverTestRunService
    {
        //ProverTestRun CreateTestRun();
        //ProverTestRun GetTestRun(Guid id);
        //IEnumerable<ProverTestRun> GetAllTestRuns();

        //Task<bool> Save(ProverTestRun testRun);
        //Task<bool> ArchiveTest(ProverTestRun testRun);
    }

    public class ProverTestRunService : IProverTestRunService
    {
        private readonly IRepository<ProverTestRun> _testRunRepository;
        private readonly IProverTestRunFactory _testRunFactory;

        public ProverTestRunService(IRepository<ProverTestRun> testRunRepository, IProverTestRunFactory testRunFactory)
        {
            _testRunRepository = testRunRepository;
            _testRunFactory = testRunFactory;
        }

        public async Task<ProverTestRun> CreateTestRun(IInstrumentFactory instrumentFactory)
        {
            return await _testRunFactory.Create(instrumentFactory);
        }

        public async Task<ProverTestRun> GetTestRun(Guid id)
        {
            return await _testRunRepository.GetByIdAsync(id);
        }

        public IEnumerable<ProverTestRun> GetTestRunsNotExported()
        {
            return _testRunRepository.Query().Where(t => t.ExportedDateTime == null && t.ArchivedDateTime.HasValue == false);
        }

        public IEnumerable<ProverTestRun> GetTestRunsArchived()
        {
            return _testRunRepository.Query().Where(t => t.ArchivedDateTime.HasValue);
        }

        public IEnumerable<ProverTestRun> GetTestRuns()
        {
            return _testRunRepository.Query();
        }

        public async Task<bool> Save(ProverTestRun testRun)
        {
            return await _testRunRepository.UpsertAsync(testRun) != null;
        }

        public async Task<bool> ArchiveTest(ProverTestRun testRun)
        {
            var updatedTestRun = await GetTestRun(testRun.Id);
            if (updatedTestRun != null)
            {
                updatedTestRun.ArchivedDateTime = DateTime.UtcNow;
                return await _testRunRepository.UpsertAsync(testRun) != null;
            }

            return false;
        }
    }

    public class ProverTestPointService
    {
    }
}
