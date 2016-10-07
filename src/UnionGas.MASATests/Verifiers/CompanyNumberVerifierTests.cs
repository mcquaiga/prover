using Moq;
using NUnit.Framework;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;

namespace UnionGas.MASATests.Verifiers
{
    [TestFixture()]
    public class CompanyNumberVerifierTests
    {
        private Mock<EvcCommunicationClient> commClient;
        private Mock<ICommPort> commPort;

        [SetUp]
        protected void Setup()
        {
            commPort = new Mock<ICommPort>();
            commClient = new Mock<EvcCommunicationClient>(commPort);
        }

        [Test()]
        public void CompanyNumberVerifierTest()
        {            

            Assert.Pass();
        }

        [Test()]
        public void VerifyTest()
        {
            Assert.Fail();
        }
    }
}