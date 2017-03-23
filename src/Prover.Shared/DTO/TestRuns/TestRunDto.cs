using System;
using System.Collections.Generic;
using Prover.Shared.Common;
using Prover.Shared.Enums;

namespace Prover.Shared.DTO.TestRuns
{
    public interface ITestRun
    {
        DateTime TestDateTime { get; set; }
        DateTime? ExportedDateTime { get; set; }
        DateTime? ArchivedDateTime { get; set; }
        bool? EventLogPassed { get; set; }
        bool? CommPortsPassed { get; set; }
        string EmployeeId { get; set; }
        string JobId { get; set; }
        Dictionary<string, string> ItemValues { get; set; }
        ICollection<TestPointDto> TestPoints { get; set; }
    }

    public class TestRunDto : Entity, ITestRun
    {
        public DateTime TestDateTime { get; set; }
        public DateTime? ExportedDateTime { get; set; }
        public DateTime? ArchivedDateTime { get; set; }
        
        public string EmployeeId { get; set; }
        public string JobId { get; set; }
        public Dictionary<string, string> ItemValues { get; set; }
        public ICollection<TestPointDto> TestPoints { get; set; }

        public bool? EventLogPassed { get; set; }
        public bool? CommPortsPassed { get; set; }

        public TestRunDto(DateTime testDateTime, 
                            DateTime? exportedDateTime, 
                            DateTime? archivedDateTime,
                            bool? eventLogPassed, 
                            bool? commPortsPassed, 
                            string employeeId, 
                            string jobId,
                            Dictionary<string, string> itemValues, 
                            ICollection<TestPointDto> testPoints)
        {
            TestDateTime = testDateTime;
            ExportedDateTime = exportedDateTime;
            ArchivedDateTime = archivedDateTime;
            EventLogPassed = eventLogPassed;
            CommPortsPassed = commPortsPassed;
            EmployeeId = employeeId;
            JobId = jobId;
            ItemValues = itemValues;
            TestPoints = testPoints;
        }

        protected TestRunDto()
        {
        }
    }
}