using System;
using System.Threading.Tasks;
using AutoMapper;
using Prover.Domain.Models.TestRuns;
using Prover.Shared.DTO.TestRuns;
using Prover.Shared.Storage;

namespace Prover.Domain.Services
{
    public class TestRunService
    {
        private readonly IRepository<TestRunDto> _testRunRepository;

        public TestRunService(IRepository<TestRunDto> testRunRepository)
        {
            _testRunRepository = testRunRepository;
        }

        public async Task<TestRun> GetTestRun(Guid id)
        {
            var testRunDto = await _testRunRepository.Get(id);
            return Mapper.Map<TestRun>(testRunDto);
        }
    }
}
