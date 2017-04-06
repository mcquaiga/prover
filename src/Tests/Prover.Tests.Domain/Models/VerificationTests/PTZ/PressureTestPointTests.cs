using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prover.Domain.Models.Instruments.Items;
using Prover.Domain.Models.VerificationTests.PTZ;

namespace Prover.Tests.Domain.Models.VerificationTests.PTZ
{
    [TestClass]
    public class PressureTestPointTests
    {
        private MockRepository _mockRepo = new MockRepository(MockBehavior.Default);

        [TestInitialize]
        public void TestInitialize()
        {
            
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockRepo.VerifyAll();
        }

        [TestMethod]
        public void Verify_Pressure_PSIA()
        {
            var pressure = _mockRepo.Create<IPressureItems>();
            pressure.Setup(p => p.TransducerType).Returns("Absolute");
            pressure.Setup(p => p.Base).Returns(14.73m);
            pressure.Setup(p => p.Factor).Returns(54.2813m);

            var pressureTestPoint = CreatePressureTestPoint(pressure, 0.8m * 1000);
            pressureTestPoint.SetGaugeValues(785.9m, 14.10m);

            Assert.AreEqual(-0.05m, pressureTestPoint.PercentError ?? -100);
        }

        [TestMethod]
        public void Verify_Pressure_PSIG()
        {
            var pressure = _mockRepo.Create<IPressureItems>();
            pressure.Setup(p => p.AtmPressure).Returns(14.7m);
            pressure.Setup(p => p.TransducerType).Returns("Gauge");
            pressure.Setup(p => p.Base).Returns(14.73m);
            pressure.Setup(p => p.Factor).Returns(55.308893m);

            var pressureTestPoint = CreatePressureTestPoint(pressure, 0.8m * 1000);
            pressureTestPoint.SetGaugeValues(800m, 0m);

            Assert.AreEqual(0.00m, pressureTestPoint.PercentError ?? -100);
        }

        private PressureTestPoint CreatePressureTestPoint(Mock<IPressureItems> pressureMock, decimal gasGauge)
        {
            return new PressureTestPoint(
                gasGauge,
                pressureMock.Object);
        }
    }
}