using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prover.Domain.Model.Instrument;
using Prover.Domain.Model.Verifications;
using Prover.Shared.Domain;
using Prover.Shared.Enums;

namespace Prover.Storage.EF.Tests
{
    public static class TestHelper
    {
        public static EvcInstrument GetTestInstrument<T>(this PersistenceTest<T> test) where T : Entity
        {
            var instrument = new EvcInstrument()
            {
                InstrumentType = "Mini-Max",
                Items = test.GetTestItemData()
            };

            return instrument;
        }

        public static VerificationRun GetTestVerificationRun<T>(this PersistenceTest<T> test) where T : Entity
        {
            var testRun = new VerificationRun
            {
                Instrument = test.GetTestInstrument(),
                TestPoints = new List<VerificationRunTestPoint>()
            };
            return testRun;
        }

        public static List<VerificationRunTestPoint> GetTestVerificationTestPoint<T>(this PersistenceTest<T> test) where T : Entity
        {
            return new List<VerificationRunTestPoint>
            {
                test.GetTestVerificationTestPoint(TestLevel.Level1),
                test.GetTestVerificationTestPoint(TestLevel.Level2),
                test.GetTestVerificationTestPoint(TestLevel.Level3)
            };
        }

        public static VerificationRunTestPoint GetTestVerificationTestPoint<T>(this PersistenceTest<T> test, TestLevel level, 
            bool createPressure = true, bool createTemp = true, bool createVolume = true) where T : Entity
        {
            var pressure = default(PressureTest);
            if (createPressure)
            {
                pressure = CreatePressureTest(PressureUnits.PSIG, PressureTransducerType.Gauge);
            }

            var temperature = default(TemperatureTest);
            if (createTemp)
            {
                temperature = new TemperatureTest
                {
                    GaugeTemperature = 90
                };
            }

            var volume = default(VolumeTest);
            if (level == TestLevel.Level1 && createVolume)
                volume = CreateVolumeTest<T>();

            return new VerificationRunTestPoint(level)
            {
                Pressure = pressure,
                Temperature = temperature,
                Volume = volume
            };
        }

        public static VolumeTest CreateVolumeTest<T>(DriveTypeDescripter driveType = DriveTypeDescripter.Rotary) where T : Entity
        {
            if (driveType == DriveTypeDescripter.Mechanical)
            {
                return new MechanicalVolumeTest
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

            return new RotaryVolumeTest()
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
                MeterDisplacement = 0.02222,
                MeterModel = "Roots3M",
                MeterModelId = 0
            };
        }

        public static PressureTest CreatePressureTest(PressureUnits units = PressureUnits.PSIA, PressureTransducerType pressureTransducerType = PressureTransducerType.Absolute)
        {
            return new PressureTest
            {
                AtmosphericPressure = 14.73,
                GaugePressure = 80,
                Factor = 1.999,
                Base = 14.73,
                GasPressure = 20,
                Range = 100,
                TransducerType = pressureTransducerType,
                Units = units,
                UnsqrFactor = 1.2999
            };
        }

        public static Dictionary<string, string> GetTestItemData<T>(this PersistenceTest<T> test) where T : Entity
        {
            var itemJson =
                "{\"0\":\" 0000098\",\"2\":\" 0008169\",\"5\":\"00000000\",\"6\":\"00000000\",\"8\":\"   80.06\",\"10\":\"  100.00\",\"11\":\"   -1.00\",\"12\":\" 14.4000\",\"13\":\" 14.6500\",\"14\":\" 14.4303\",\"26\":\"   32.30\",\"27\":\"  -30.00\",\"28\":\"  170.00\",\"34\":\"   60.00\",\"35\":\" -0.0543\",\"44\":\"  6.4499\",\"45\":\"  1.0563\",\"47\":\"  1.0069\",\"53\":\"  0.5850\",\"54\":\"  1.6000\",\"55\":\"  0.7000\",\"56\":\"  2.0000\",\"57\":\"  2.0000\",\"62\":\"12019164\",\"87\":\"       0\",\"89\":\"       0\",\"90\":\"       6\",\"92\":\"       5\",\"93\":\"       0\",\"94\":\"       2\",\"98\":\"       3\",\"109\":\"       0\",\"110\":\"       0\",\"111\":\"       0\",\"112\":\"       0\",\"113\":\"098.8073\",\"122\":\"  6.9200\",\"137\":\"  100.00\",\"140\":\" 0000098\",\"141\":\"       1\",\"142\":\" 1000.00\",\"147\":\"       0\",\"200\":\"12019164\",\"201\":\"02549645\"}";
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(itemJson);
        }
    }
}
