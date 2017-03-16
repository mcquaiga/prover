using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prover.Domain.Models.TestRuns;

namespace Prover.Tests.Domain.Models.TestRuns
{
    [TestClass]
    public class TestRunTests
    {
        private MockRepository mockRepository;



        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);


        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.mockRepository.VerifyAll();
        }

        [TestMethod]
        public void TestMethod1()
        {
            TestRun testRun = this.CreateTestRun();


        }

        private TestRun CreateTestRun()
        {
            return new TestRun();
        }
    }
}