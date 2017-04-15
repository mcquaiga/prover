using Moq;
using NUnit.Framework;
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
            //Mapper.Initialize(MappingConfiguration.Configure);
        }

        [Test()]
        public void Verify_Pressure_PSIA()
        {
            var pressureTestPoint = CreatePSIATest();

            Assert.AreEqual(54.3109, pressureTestPoint.CalculatedFactor(), 0.001);
            Assert.AreEqual(-0.05m, pressureTestPoint.PercentError() ?? -100);
        }

        [Test]
        public void Verify_Pressure_PSIG()
        {
            var pressureTestPoint = CreatePressureTestPoint();

            Assert.AreEqual(55.3088, pressureTestPoint.CalculatedFactor(), 0.001);
            Assert.AreEqual(0.00m, pressureTestPoint.PercentError() ?? -100);
        }
      
        private PressureTest CreatePressureTestPoint()
        {
            return new PressureTest()
            {
                AtmosphericPressure = 14.73,
                Base = 14.73,
                Factor = 55.3088,
                GasPressure = 800,
                GaugePressure = 800,
                TransducerType = PressureTransducerType.Gauge,
                Units = PressureUnits.PSIG,
                Range = 1000,
                UnsqrFactor = 1
            };
        }

        private PressureTest CreatePSIATest()
        {
            return new PressureTest()
            {
                AtmosphericPressure = 14.73,
                Base = 14.73,
                Factor = 54.3109,
                GasPressure = 800,
                GaugePressure = 785.27,
                TransducerType = PressureTransducerType.Absolute,
                Units = PressureUnits.PSIA,
                Range = 1000,
                UnsqrFactor = 1
            };
        }
    }
}