namespace Prover.Domain.Verification.TestRun
{
    public class TestRun : AggregateRoot<Guid>
    {
        public TestRun(IInstrument instrument) : base((Guid) Guid.NewGuid())
        {
            Instrument = instrument;
            TestDateTime = DateTime.UtcNow;

            TestPoints = new List<TestPoint>();
            foreach (TestLevel testLevel in Enum.GetValues(typeof(TestLevel)))
                TestPoints.Add(new TestPoint(instrument, testLevel));
        }

        public DateTime? ArchivedDateTime { get; set; }
        public DateTime? ExportedDateTime { get; set; }

        public IInstrument Instrument { get; set; }
        public DateTime TestDateTime { get; set; }
        public ICollection<TestPoint> TestPoints { get; set; }
    }
}

//public bool? CommPortsPassed { get; set; }
//public string EmployeeId { get; set; }
//public bool? EventLogPassed { get; set; }
//public string JobId { get; set; }