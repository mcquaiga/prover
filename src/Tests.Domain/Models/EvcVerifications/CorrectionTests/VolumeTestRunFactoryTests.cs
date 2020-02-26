using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prover.Domain.EvcVerifications.Verifications.Volume.InputTypes;

namespace Tests.Domain.Models.EvcVerifications.CorrectionTests
{
    [TestClass()]
    public class VolumeTestRunFactoryTests
    {
        [TestMethod()]
        public void CreateTest()
        {
            var mock = new Mock<IVolumeInputType>();
      
        }

        [TestMethod()]
        public void UpdateTest()
        {
            Assert.Fail();
        }
    }
}