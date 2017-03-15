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

        public Dictionary<string, string> ItemValues { get; set; }
        public ICollection<TestPoint> TestPoints { get; set; }

        public static TestRun Create(IInstrument instrument, Dictionary<string, string> itemValues)
        {
            var testRun = new TestRun()
            {
                TestDateTime = DateTime.Now,

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
                var testPoint = new TestPoint();
                switch (testRun.CorrectorType)
                {
                    case EvcCorrectorType.T:
                        testPoint.Temperature = new TemperatureTestPoint();
                        break;
                    case EvcCorrectorType.P:
                        testPoint.Pressure =  new PressureTestPoint();
                        break;
                    case EvcCorrectorType.PTZ:
                        testPoint.Pressure = new PressureTestPoint();
                        testPoint.Temperature = new TemperatureTestPoint();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (i == 0)
                {
                    testPoint.Volume = new VolumeTestPoint(testRun.CorrectorType);
                }
                testRun.TestPoints.Add(testPoint);
            }
            

            return testRun;
        }
    }
}