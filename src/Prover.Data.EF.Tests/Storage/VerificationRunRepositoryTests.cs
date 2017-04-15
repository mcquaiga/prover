using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Prover.Data.EF.Repositories;
using Prover.Data.EF.Storage;
using Prover.Domain.Model.Instrument;
using Prover.Domain.Model.Verifications;
using Prover.Shared.Enums;

namespace Prover.Data.EF.Tests.Storage
{
    [TestFixture()]
    public class VerificationRunRepositoryTests : PersistenceTest<VerificationRun>
    {
        
        [SetUp]
        public void Init()
        {
            
        }

        protected override EfRepository<VerificationRun> SetRepository()
        {
            return new VerificationRunRepository(GetContext());
        }

        [Test()]
        public async Task Can_save_and_load_test_run()
        {
            var testRun = GetTestVerificationRun();

            var dbTestRun = await SaveAndLoadEntity(testRun);

            Assert.IsNotNull(dbTestRun);
            dbTestRun.PropertiesShouldEqual(GetTestVerificationRun(), new []{ "TestDateTime" });
            Assert.IsNotNull(dbTestRun.Instrument);
            dbTestRun.Instrument.PropertiesShouldEqual(GetTestInstrument());
            Assert.IsNotEmpty(dbTestRun.TestPoints);
        }

        [Test]
        public async Task Can_update_test_run()
        {
            var testRun = GetTestVerificationRun();
            var dbRun = await SaveAndLoadEntity(testRun);

            var now = DateTime.UtcNow;
            testRun.ExportedDateTime = now;
            dbRun.ExportedDateTime = now;

            dbRun = await SaveAndLoadEntity(dbRun);
            dbRun.PropertiesShouldEqual(testRun, "TestDateTime");
        }

        private VerificationRun GetTestVerificationRun()
        {
            var points = new List<VerificationRunTestPoint>()
            {
                {GetTestVerificationTestPoint(TestLevel.Level1)},
                {GetTestVerificationTestPoint(TestLevel.Level2)},
                {GetTestVerificationTestPoint(TestLevel.Level3)}
            };

            var testRun = new VerificationRun()
            {
                Instrument = GetTestInstrument(),
                TestPoints = points
            };
            return testRun;
        }
        

        private VerificationRunTestPoint GetTestVerificationTestPoint(TestLevel level)
        {
            var pressure = new PressureTest()
            {
                AtmosphericPressure = 14.73,
                GaugePressure = 80,
                Factor = 1.999,
                Base = 14.73,
                GasPressure = 20,
                Range = 100,
                TransducerType = PressureTransducerType.Absolute,
                Units = PressureUnits.PSIA,
                UnsqrFactor = 1.2999
            };

            var temperature = new TemperatureTest()
            {
                GaugeTemperature = 90
            };

            var volume = default(VolumeTest);
            if (level == TestLevel.Level1)
            {
                volume = new MechanicalVolumeTest()
                {
                    AppliedInput = 100,
                    PulserA = 100,
                    PulserB = 200,
                    CorrectedStartReading = 213.7072,
                    CorrectedEndReading = 280.8001,
                    CorrectedMultiplier = 1000,

                    UncorrectedStartReading = 588405,
                    UncorrectedEndReading = 588502,
                    UncorrectedMultiplier = 100,

                    DriveRate = 100,
                    EnergyStartReading = 0,
                    EnergyEndReading = 671,
                    GasEnergyTotal = 671,
                    EnergyUnits = "Therms"
                };
            }

            return new VerificationRunTestPoint(level)
            {
                Pressure = pressure,
                Temperature = temperature,
                Volume = volume
            };
        }
    }
}