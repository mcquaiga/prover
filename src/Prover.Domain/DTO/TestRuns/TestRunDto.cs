using System;
using System.Collections.Generic;
using Prover.Shared.Common;
using Prover.Shared.DTO.Instrument;
using Prover.Shared.Enums;

namespace Prover.Shared.DTO.TestRuns
{
    public interface ITestRun
    {
        EvcCorrectorType CorrectorType { get; set; }
        InstrumentTypeDto InstrumentType { get; set; }
        string SerialNumber { get; set; }
        string InventoryNumber { get; set; }
        DateTime TestDateTime { get; set; }
        DateTime? ExportedDateTime { get; set; }
        DateTime? ArchivedDateTime { get; set; }
        bool? EventLogPassed { get; set; }
        bool? CommPortsPassed { get; set; }
        string EmployeeId { get; set; }
        string JobId { get; set; }
        ICollection<EvcItemValueDto> ItemValues { get; set; }
        ICollection<TestPointDto> TestPoints { get; set; }
    }

    public class TestRunDto : Entity, ITestRun
    {
        public EvcCorrectorType CorrectorType { get; set; }
        public InstrumentTypeDto InstrumentType { get; set; }
        public string SerialNumber { get; set; }
        public string InventoryNumber { get; set; }

        public DateTime TestDateTime { get; set; }
        public DateTime? ExportedDateTime { get; set; }
        public DateTime? ArchivedDateTime { get; set; }

        public bool? EventLogPassed { get; set; }
        public bool? CommPortsPassed { get; set; }

        public string EmployeeId { get; set; }
        public string JobId { get; set; }

        public ICollection<EvcItemValueDto> ItemValues { get; set; }

        public ICollection<TestPointDto> TestPoints { get; set; }

        public TestRunDto(EvcCorrectorType correctorType, InstrumentTypeDto instrumentType, string serialNumber,
            string inventoryNumber, DateTime testDateTime, DateTime? exportedDateTime, DateTime? archivedDateTime,
            bool? eventLogPassed, bool? commPortsPassed, string employeeId, string jobId,
            ICollection<EvcItemValueDto> instrumentItems, ICollection<TestPointDto> testPoints)
        {
            CorrectorType = correctorType;
            InstrumentType = instrumentType;
            SerialNumber = serialNumber;
            InventoryNumber = inventoryNumber;
            TestDateTime = testDateTime;
            ExportedDateTime = exportedDateTime;
            ArchivedDateTime = archivedDateTime;
            EventLogPassed = eventLogPassed;
            CommPortsPassed = commPortsPassed;
            EmployeeId = employeeId;
            JobId = jobId;
            ItemValues = instrumentItems;
            TestPoints = testPoints;
        }

        protected TestRunDto()
        {
        }
    }
}