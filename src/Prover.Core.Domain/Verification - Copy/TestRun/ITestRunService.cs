namespace Prover.Domain.Verification.TestRun
{
    public interface ITestRunService
    {
        Task<bool> ArchiveTest(TestRun testRun);
        IEnumerable<TestRun> FindTests(Func<TestRun, bool> searchPredicate);
        IEnumerable<TestRun> GetAllTests(bool includeExported = false, bool includeArchived = false);
        TestRun GetTest(Guid id);

        Task<bool> SaveTest(TestRun testRun);
    }
}