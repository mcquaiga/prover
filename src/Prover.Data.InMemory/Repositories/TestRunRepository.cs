using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Prover.Data.InMemory.Database;
using Prover.Domain.Verification.TestRun;
using Prover.Shared.Storage;

namespace Prover.Data.InMemory.Repositories
{
    internal class TestRunRepository : Repository<TestRun, Guid, TestRunDao>, ITestRunRepository
    {
        public TestRunRepository(InMemoryDataContextFactory contextFactory) : base(contextFactory)
        {
        }

        public override TestRun FindBy(Guid id)
        {
            var databaseObject = ContextFactory.Create().TestRuns.FirstOrDefault(t => t.Id == id);
            if (databaseObject != null)
            {
                return ConvertToDomainType(databaseObject);
            }
        }

        private TestRun ConvertToDomainType(TestRunDao databaseObject)
        {
            return new TestRun();
        }

        public override TestRunDao ConvertToDatabaseType(TestRun domainType)
        {
            return new TestRunDao()
            {
                TestDateTime = domainType.TestDateTime,
                ExportedDateTime = domainType.ExportedDateTime,
                ArchivedDateTime = domainType.ArchivedDateTime,
                InstrumentType = domainType.Instrument.GetType().AssemblyQualifiedName,
                ItemValues = JsonConvert.SerializeObject(domainType.Instrument.ItemData),
            };
        }
    }

    internal class TestRunDao
    {
        public Guid Id { get; set; }

        public string InstrumentType { get; set; }

        public DateTime TestDateTime { get; set; }
        public DateTime? ExportedDateTime { get; set; }
        public DateTime? ArchivedDateTime { get; set; }
        public string ItemValues { get; set; }

        public virtual ICollection<TestPointDao> TestPoints { get; set; }
    }

    internal class TestPointDao
    {
        public Guid Id { get; set; }
        public int Level { get; set; }

        public Guid TestRunId { get; set; }
        public virtual TestRunDao TestRun { get; set; }

        public Guid? PressureId { get; set; }
        public virtual PressureTestDao Pressure { get; set; }

        public Guid? TemperatureId { get; set; }
        public virtual TemperatureTestDao Temperature { get; set; }

        public Guid? VolumeId { get; protected set; }
        public virtual VolumeTestDao Volume { get; set; }
    }

    internal class PressureTestDao
    {
        public Guid Id { get; set; }

        public Guid TestPointId { get; set; }
        public virtual TestPointDao TestPoint { get; set; }

        public decimal Gauge { get; set; }
        public decimal? AtmosphericGauge { get; set; }
        public string ItemData { get; set; }
    }

    
    internal class TemperatureTestDao
    {
        public Guid Id { get; set; }

        public Guid TestPointId { get; set; }
        public virtual TestPointDao TestPoint { get; set; }

        public decimal Gauge { get; set; }
        public string ItemData { get; set; }
    }

    internal class VolumeTestDao
    {
        public Guid Id { get; set; }

        public Guid TestPointId { get; set; }
        public virtual TestPointDao TestPoint { get; set; }

        public int PulserACount { get; set; }
        public int PulserBCount { get; set; }
        public int PulserCCount { get; set; }
        public decimal AppliedInput { get; set; }

        public string PreTestItemData { get; set; }
        public string PostTestItemData { get; set; }
    }
}
