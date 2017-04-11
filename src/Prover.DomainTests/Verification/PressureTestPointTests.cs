using System.Collections.Generic;
using AutoMapper;
using Moq;
using NUnit.Framework;
using Prover.Domain.Instrument.Items;
using Prover.Domain.Verification.TestPoints.Pressure;
using Prover.Shared.DTO.TestRuns;
using Prover.Shared.Enums;

namespace Prover.Domain.Tests.Verification
{
    [TestFixture()]
    public class PressureTestPointTests
    {
        private readonly MockRepository _mockRepo = new MockRepository(MockBehavior.Default);

        [PostTest]
        public void TestCleanup()
        {
            _mockRepo.VerifyAll();
        }

        [SetUp]
        public void TestInitialize()
        {
            Mapper.Initialize(MappingConfiguration.Configure);
        }

        [Test()]
        public void Verify_Pressure_PSIA()
        {
            var pressure = _mockRepo.Create<IPressureItems>();
            pressure.Setup(p => p.TransducerType).Returns("Absolute");
            pressure.Setup(p => p.Base).Returns(14.73);
            pressure.Setup(p => p.Factor).Returns(54.2813);

            var pressureTestPoint = CreatePressureTestPoint(pressure);
            pressureTestPoint.SetGaugeValues(785.9, 14.10);

            Assert.AreEqual(54.3109, pressureTestPoint.ActualFactor, 0.001);
            Assert.AreEqual(-0.05m, pressureTestPoint.PercentError ?? -100);
        }

        [Test]
        public void Verify_Pressure_PSIG()
        {
            var pressure = _mockRepo.Create<IPressureItems>();
            pressure.Setup(p => p.AtmPressure).Returns(14.7);
            pressure.Setup(p => p.TransducerType).Returns("Gauge");
            pressure.Setup(p => p.Base).Returns(14.73);
            pressure.Setup(p => p.Factor).Returns(55.3088);

            var pressureTestPoint = CreatePressureTestPoint(pressure);
            pressureTestPoint.SetGaugeValues(800, 0);

            Assert.AreEqual(55.3088, pressureTestPoint.ActualFactor, 0.001);
            Assert.AreEqual(0.00m, pressureTestPoint.PercentError ?? -100);
        }
      
        private PressureTestPoint CreatePressureTestPoint(Mock<IPressureItems> pressureMock)
        {
            return new PressureTestPoint()
            {
                EvcItems = pressureMock.Object
            };
        }
    }
}