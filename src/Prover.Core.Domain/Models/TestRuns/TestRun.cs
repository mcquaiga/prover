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

        public Dictionary<string, string> ItemData { get; set; }
        public Dictionary<TestLevel, TestPoint> TestPoints { get; set; }

        public static TestRun Create(IInstrument instrument)
        {
            var testRun = new TestRun
            {
                TestDateTime = DateTime.Now,
                ExportedDateTime = null,
                ArchivedDateTime = null,

                Instrument = instrument,
                ItemData = instrument.ItemData,

                SerialNumber = instrument.SiteInformationItems.SerialNumber,
                InventoryNumber = instrument.SiteInformationItems.CompanyNumber,
                FirmwareVersion = instrument.SiteInformationItems.FirmwareVersion,

                CorrectorType = instrument.CorrectorType(),
                TestPoints = new Dictionary<TestLevel, TestPoint>()
            };

            for (var i = 0; i < 3; i++)
            {
                var testLevel = (TestLevel) i;
                var testPoint = TestPoint.Create(testLevel, instrument);
                testRun.TestPoints.Add(testLevel, testPoint);
            }

            return testRun;
        }
    }
}