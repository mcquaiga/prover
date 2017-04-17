using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Shared.DTO.TestRuns;
using Prover.Shared.Storage;

namespace Prover.Core.Services.TestRuns
{
    public class TestRunService
    {
        private readonly IRepository<TestRunDto> _testRunRepository;

        public TestRunService(IRepository<TestRunDto> testRunRepository)
        {
            _testRunRepository = testRunRepository;
        }

        public async Task<TestRunDto> GetTestRun(Guid id)
        {
            return await _testRunRepository.Get(id);
        }
    }
}
