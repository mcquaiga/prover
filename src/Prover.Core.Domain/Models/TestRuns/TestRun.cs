using System;
using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Instruments;
using Prover.CommProtocol.Common.Items;
using Prover.Shared.Enums;

namespace Prover.Domain.Models.TestRuns
{
    public class TestRun
    {
        public EvcCorrectorType CorrectorType { get; set; }
        public IInstrument Instrument { get; set; }
        public string SerialNumber { get; set; }
        public string InventoryNumber { get; set; }
        public string FirmwareVersion { get; set; }
        public DateTime TestDateTime { get; set; }
        public DateTime? ExportedDateTime { get; set; }
        public DateTime? ArchivedDateTime { get; set; }
        public bool? EventLogPassed { get; set; }
        public bool? CommPortsPassed { get; set; }
        public string EmployeeId { get; set; }
        public string JobId { get; set; }

        public ICollection<ItemValue> ItemValues { get; set; }
        public ICollection<TestPoint> TestPoints { get; set; }

        public static TestRun Create(IInstrument instrument, List<ItemValue> itemValues)
        {
            var testRun = new TestRun
            {
                TestDateTime = DateTime.Now,
                ExportedDateTime = null,
                ArchivedDateTime = null,

                Instrument = instrument,
                ItemValues = itemValues,

                SerialNumber = instrument.GetItemValue(ItemCodes.SiteInfo.SerialNumber, itemValues).Description,
                InventoryNumber = instrument.GetItemValue(ItemCodes.SiteInfo.CompanyNumber, itemValues).Description,
                FirmwareVersion = instrument.GetItemValue(ItemCodes.SiteInfo.Firmware, itemValues).RawValue,

                CorrectorType = instrument.CorrectorType(itemValues),
                TestPoints = new List<TestPoint>(),
            };

            for (var i = 0; i < 3; i++)
            {
                var testLevel = (TestLevel) i;
                var testPoint = TestPoint.Create(testLevel, instrument, itemValues);
                testRun.TestPoints.Add(testPoint);
            }

            return testRun;
        }
    }
}