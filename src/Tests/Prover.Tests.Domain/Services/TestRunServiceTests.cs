using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prover.Domain.Services;
using Prover.Shared.DTO.TestRuns;
using Prover.Shared.Storage;

namespace Prover.Tests.Domain.Services
{
    [TestClass]
    public class TestRunServiceTests
    {
        private MockRepository mockRepository;

        private Mock<IRepository<TestRunDto>> _mockRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            _mockRepository = mockRepository.Create<IRepository<TestRunDto>>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.mockRepository.VerifyAll();
        }

        [TestMethod]
        public void TestMethod1()
        {
            TestRunService service = this.CreateService();

        }

        private TestRunService CreateService()
        {
            return new TestRunService(
                _mockRepository.Object);
        }
    }
}