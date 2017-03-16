using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prover.Domain.DriveTypes;
using Prover.Domain.Models.TestRuns;
using Prover.Shared.Enums;

namespace Prover.Tests.Domain.Models.TestRuns
{
    [TestClass]
    public class VolumeTestPointTests
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
        public void Rotary_Temperature_Only_Volume_Test()
        {
            var volume = CreateRotaryVolumeTestPoint(EvcCorrectorType.T);
            Assert.IsTrue(volume.Passed);
        }

        private VolumeTestPoint CreateRotaryVolumeTestPoint(EvcCorrectorType correctorType)
        {
            var meterIndexInfo = new MeterIndexInfo(
                description: "Roots7M", 
                id: 4,
                meterDisplacement: 0.0625000m,
                unCorPulsesX10: 65,
                unCorPulsesX100: 7);

            var drive = new RotaryDrive(meterIndexInfo, 0.062500m, 100);

            return new VolumeTestPoint(correctorType)
            {
                AppliedInput = 10410,
                CorrectedStart = 271.5214m,
                CorrectedEnd = 278.3972m,
                UncorrectedStart = 2667.8001m,
                UncorrectedEnd = 2732.8001m,
                PulseACount = 7,
                PulseBCount = 65,
                TemperatureFactor = 1.0568m,
                DriveType = drive,
                UnCorrectedMultiplier = 10,
                CorrectedMultiplier = 100
            };
        }
    }
}