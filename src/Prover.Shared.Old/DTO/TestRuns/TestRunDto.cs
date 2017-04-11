using System;
using System.Collections.Generic;
using Prover.Shared.Domain;

namespace Prover.Shared.DTO.TestRuns
{
    public class TestRunDto : Entity<Guid>
    {
        public string Instrument { get; set; }
        public DateTime TestDateTime { get; set; }
        public DateTime? ArchivedDateTime { get; set; }
        public DateTime? ExportedDateTime { get; set; }
        public Dictionary<string, string> ItemData { get; set; }
        public ICollection<TestPointDto> TestPoints { get; set; }
        
        protected TestRunDto(Guid id) : base(id)
        {
        }

        public TestRunDto(Guid id, string instrument, DateTime testDateTime, DateTime? archivedDateTime, DateTime? exportedDateTime, Dictionary<string, string> itemData, ICollection<TestPointDto> testPoints)
            : base(id)
        {
            Instrument = instrument;
            TestDateTime = testDateTime;
            ArchivedDateTime = archivedDateTime;
            ExportedDateTime = exportedDateTime;
            ItemData = itemData;
            TestPoints = testPoints;
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