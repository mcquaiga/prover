using AutoMapper;
using Moq;
using NUnit.Framework;
using Prover.Domain.Instrument;
using Prover.Domain.Instrument.Items;
using Prover.Shared.Enums;

namespace Prover.Domain.Tests.Verification.TestRun
{
    [TestFixture()]
    public class TestRunMappingTests
    {
        private readonly MockRepository _mockRepo = new MockRepository(MockBehavior.Default);

        [SetUp]
        public void Setup()
        {
            Mapper.Initialize(MappingConfiguration.Configure);
        }

        [PostTest]
        public void TestCleanup()
        {
            _mockRepo.VerifyAll();
        }

        [Test()]
        public void Test_Mapping_Configuration_IsValid()
        {
            Mapper.Configuration.AssertConfigurationIsValid();
        }

        [Test]
        public void Verify_TestRun_Mapping()
        {
            var instrument = StubInstrument();
            var testRun = new Domain.Verification.TestRun.TestRun(instrument.Object);
        }

        public Mock<IInstrument> StubInstrument()
        {
            var site = _mockRepo.Create<ISiteInformationItems>()
                .SetupGet(p => p.SerialNumber).Returns("000001");

            var instrument = _mockRepo.Create<IInstrument>();
            
            instrument.SetupGet(p => p.CorrectorType).Returns(EvcCorrectorType.PTZ);
            //instrument.SetupGet(p => p.SiteInformationItems).Returns()
            return instrument;
        }
    }
}