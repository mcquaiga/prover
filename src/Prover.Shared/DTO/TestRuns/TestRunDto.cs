using System;
using System.Collections.Generic;
using Prover.Shared.Domain;

namespace Prover.Shared.DTO.TestRuns
{
    public class TestRunDto : Entity<Guid>
    {
        protected TestRunDto()
        {
        }

        public DateTime? ArchivedDateTime { get; set; }
        public bool? CommPortsPassed { get; set; }

        public string EmployeeId { get; set; }

        public bool? EventLogPassed { get; set; }
        public DateTime? ExportedDateTime { get; set; }
        public string Instrument { get; set; }
        public Dictionary<string, string> ItemData { get; set; }
        public string JobId { get; set; }

        public DateTime TestDateTime { get; set; }

        public ICollection<TestPointDto> TestPoints { get; set; }

        protected override void Validate()
        {
            throw new NotImplementedException();
        }
    }
}


//public TestRunDto(DateTime testDateTime, 
//                    DateTime? exportedDateTime, 
//                    DateTime? archivedDateTime,
//                    bool? eventLogPassed, 
//                    bool? commPortsPassed, 
//                    string employeeId, 
//                    string jobId,
//                    Dictionary<string, string> itemValues, 
//                    ICollection<TestPointDto> testPoints)
//{
//    TestDateTime = testDateTime;
//    ExportedDateTime = exportedDateTime;
//    ArchivedDateTime = archivedDateTime;
//    EventLogPassed = eventLogPassed;
//    CommPortsPassed = commPortsPassed;
//    EmployeeId = employeeId;
//    JobId = jobId;
//    ItemValues = itemValues;
//    TestPoints = testPoints;
//}