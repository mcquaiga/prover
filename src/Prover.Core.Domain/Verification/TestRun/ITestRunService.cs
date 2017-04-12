using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Shared.DTO.TestRuns;

namespace Prover.Domain.Verification.TestRun
{
    public interface ITestRunService
    {
        TestRun GetTest(Guid id);
        IEnumerable<TestRun> GetAllTests(bool includeExported = false, bool includeArchived = false);
        IEnumerable<TestRun> FindTests(Func<TestRun, bool> searchPredicate);

        Task<bool> SaveTest(TestRun testRun);
        Task<bool> ArchiveTest(TestRun testRun);
    }
}
