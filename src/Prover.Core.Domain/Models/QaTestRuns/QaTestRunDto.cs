﻿using System;
using System.Collections.Generic;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Domain.Enums;

namespace Prover.Core.Domain.Models.QaTestRuns
{
    public interface IQaTestRun
    {
        EvcCorrectorType CorrectorType { get; set; }
        InstrumentType InstrumentType { get; set; }
        string SerialNumber { get; set; }
        string InventoryNumber { get; set; }
        DateTime TestDateTime { get; set; }
        DateTime? ExportedDateTime { get; set; }
        DateTime? ArchivedDateTime { get; set; }
        bool? EventLogPassed { get; set; }
        bool? CommPortsPassed { get; set; }
        string EmployeeId { get; set; }
        string JobId { get; set; }
        ICollection<ItemValue> InstrumentItems { get; set; }
        ICollection<QaTestPointDto> TestPoints { get; set; }
    }

    public class QaTestRunDto : IQaTestRun
    {
        public QaTestRunDto(EvcCorrectorType correctorType, InstrumentType instrumentType, string serialNumber,
            string inventoryNumber, DateTime testDateTime, DateTime? exportedDateTime, DateTime? archivedDateTime,
            bool? eventLogPassed, bool? commPortsPassed, string employeeId, string jobId,
            ICollection<ItemValue> instrumentItems, ICollection<QaTestPointDto> testPoints)
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
            InstrumentItems = instrumentItems;
            TestPoints = testPoints;
        }

        protected QaTestRunDto()
        {
        }

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

        public ICollection<ItemValue> InstrumentItems { get; set; }

        public ICollection<QaTestPointDto> TestPoints { get; set; }
    }
}