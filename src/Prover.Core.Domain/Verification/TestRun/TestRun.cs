using System;
using System.Collections.Generic;
using AutoMapper;
using Prover.Domain.Instrument;
using Prover.Domain.Verification.TestPoints;
using Prover.Shared.Domain;
using Prover.Shared.DTO.TestRuns;
using Prover.Shared.Enums;

namespace Prover.Domain.Verification.TestRun
{
    public class TestRun : AggregateRoot<Guid>
    {
        public TestRun(IInstrument instrument) : base(Guid.NewGuid())
        {
            Instrument = instrument;
            TestDateTime = DateTime.UtcNow;

            TestPoints = new List<TestPoint>();
            foreach (TestLevel testLevel in Enum.GetValues(typeof(TestLevel)))
                TestPoints.Add(new TestPoint(instrument, testLevel));
        }

        public TestRun(Guid id, IInstrument instrument, IList<TestPoint> testPoints, DateTime? archivedDateTime, DateTime? exportedDateTime, DateTime testDateTime)
            : base(id)
        {
            Instrument = instrument;
            TestPoints = testPoints;
            ArchivedDateTime = archivedDateTime;
            ExportedDateTime = exportedDateTime;
            TestDateTime = testDateTime;
        }

        public IInstrument Instrument { get; set; }
        public IList<TestPoint> TestPoints { get; set; }

        public DateTime? ArchivedDateTime { get; set; }
        public DateTime? ExportedDateTime { get; set; }
        public DateTime TestDateTime { get; set; }

        public virtual TestRunDto ConvertToDto()
        {
            return Mapper.Map<TestRunDto>(this);
        }

        public static TestRun ConvertToDomain(TestRunDto testRunDto)
        {
            return Mapper.Map<TestRun>(testRunDto);
        }
    }
}

//public bool? CommPortsPassed { get; set; }
//public string EmployeeId { get; set; }
//public bool? EventLogPassed { get; set; }
//public string JobId { get; set; }