using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Prover.Domain.Verification.TestRun;
using Prover.Shared.DTO.TestRuns;
using Prover.Shared.Storage;

namespace Prover.Services.ServiceClients
{
    public class TestRunService : ITestRunService
    {
        private readonly IRepository<TestRunDto> _repository;

        public TestRunService(IRepository<TestRunDto> repository)
        {
            _repository = repository;
        }

        public async Task<bool> ArchiveTest(TestRun testRun)
        {
            var updated = GetTest(testRun.Id);
            if (updated == null) return false;

            testRun.ArchivedDateTime = DateTime.UtcNow;
            return await _repository.UpsertAsync(testRun.ConvertToDto()) != null;
        }

        public IEnumerable<TestRun> FindTests(Func<TestRun, bool> searchPredicate)
        {
            return _repository.Query().ProjectTo<TestRun>().Where(searchPredicate);
        }

        public IEnumerable<TestRun> GetAllTests(bool includeExported = false, bool includeArchived = false)
        {
            var searchPredicate =
                new Func<TestRun, bool>(
                    x =>
                        x.ArchivedDateTime.HasValue == includeArchived || x.ExportedDateTime.HasValue == includeExported);

            var query = _repository.Query();
            return query.ProjectTo<TestRun>().Where(searchPredicate);
        }

        public TestRun GetTest(Guid id)
        {
            return Mapper.Map<TestRun>(_repository.GetByIdAsync(id));
        }

        public async Task<bool> SaveTest(TestRun testRun)
        {
            var dto = Mapper.Map<TestRunDto>(testRun);
            return await _repository.UpsertAsync(dto) != null;
        }
    }
}