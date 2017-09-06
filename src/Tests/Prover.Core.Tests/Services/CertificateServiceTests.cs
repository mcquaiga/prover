using NUnit.Framework;
using Prover.Core.Storage;

namespace Prover.Core.Tests.Services
{
    [TestFixture()]
    public class CertificateServiceTests
    {
        private ProverContext _proverContext;

        [SetUp]
        private void Setup()
        {
            _proverContext = new ProverContext();
        }

        [Test()]
        public void GetInstrumentsWithNoCertificateTest()
        {
            
        }
    }
}