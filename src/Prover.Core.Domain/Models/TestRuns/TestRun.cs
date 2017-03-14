using Prover.CommProtocol.Common.Items;
using Prover.Domain.DriveTypes;
using Prover.Shared;
using Prover.Shared.DTO.TestRuns;
using Prover.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.MiHoneywell.Items;

namespace Prover.Domain.Models.TestRuns
{
    public class TestRunFactory
    {
        public TestRun Create(TestRunDto testRunDto)
        {
            return new TestRun();
            //var instrumentType = CommProtocol.Common.InstrumentTypes.Instruments.GetAll().FirstOrDefault(i => i.Id == testRunDto.InstrumentType.Id);

            //var itemValues = _testRunDto.ItemValues.ToDictionary(x => int.Parse(x.Id), y => y.Value);
            //ItemValues = ItemHelpers.LoadItems(InstrumentType, itemValues).ToList();

        }
    }

    public class TestRun
    {
        public EvcCorrectorType CorrectorType { get; set; }
        public InstrumentType InstrumentType { get; set; }
        public string SerialNumber { get; set; }
        public string InventoryNumber { get; set; }
        public DateTime TestDateTime { get; set; }
        public DateTime? ExportedDateTime { get; set; }
        public DateTime? ArchivedDateTime { get; set; }
        public bool? EventLogPassed { get; set; }
        public bool? CommPortsPassed { get; set; }
        public string EmployeeId { get; set; }
        public string JobId { get; set; }

        public ICollection<ItemValue> ItemValues { get; set; }
        public ICollection<TestPoint> TestPoints { get; set; }

        public VolumeTestPoint VolumeTest => TestPoints.FirstOrDefault(t => t.Volume != null)?.Volume;

        public TestRun Create(InstrumentType instrumentType, Dictionary<string, string> itemValues)
        {
            var test = new TestRun();

            ItemValues = instrumentType..LoadItems(InstrumentType, itemValues).ToList();
        }
    }

    public class TestPoint
    {
        public TestLevel Level { get; set; }
        public PressureTestPoint Pressure { get; set; }
        public TemperatureTestPoint Temperature { get; set; }
        public SuperFactorTestPoint SuperFactor { get; protected set; }
        public VolumeTestPoint Volume { get; set; }
    }

    public class VolumeTestPoint
    {
        public int PulseACount { get; set; }
        public int PulseBCount { get; set; }
        public decimal AppliedInput { get; set; }
        public IDriveType DriveType { get; protected set; }

        public ICollection<ItemValue> PreTestValues { get; set; }
        public ICollection<ItemValue> PostTestValues { get; set; }
    }

    public class SuperFactorTestPoint
    {
    }

    public class TemperatureTestPoint
    {
    }

    public class PressureTestPoint
    {
    }
}
