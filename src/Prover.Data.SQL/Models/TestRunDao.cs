using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Shared.Common;
using Prover.Shared.DTO.Instrument;
using Prover.Shared.DTO.TestRuns;
using Prover.Shared.Enums;

namespace Prover.Data.SQL.Models
{
    [Table("QA_TestRuns")]
    internal class TestRunDao : AggregateRoot
    {
        public string CorrectorType { get; set; }
        public string InstrumentType { get; set; }
        
        public DateTime TestDateTime { get; set; }
        public DateTime? ExportedDateTime { get; set; }
        public DateTime? ArchivedDateTime { get; set; }

        public bool? EventLogPassed { get; set; }
        public bool? CommPortsPassed { get; set; }

        public string EmployeeId { get; set; }
        public string JobId { get; set; }

        public string ItemValues { get; protected set; }

        public virtual ICollection<TestPointDao> TestPoints { get; set; }
    }

    [Table("QA_TestPoints")]
    internal class TestPointDao : Entity
    {
        public int Level { get; set; }

        [Required]
        public Guid TestRunId { get; set; }
        public virtual TestRunDao TestRun { get; set; }

        public Guid? PressureId { get; set; }
        public virtual PressureTestDao Pressure { get; set; }

        public Guid? TemperatureId { get; set; }
        public virtual TemperatureTestDao Temperature { get; set; }

        public Guid? VolumeId { get; protected set; }
        public virtual VolumeTestDao Volume { get; set; }
    }

    [Table("QA_PressureTests")]
    internal class PressureTestDao : Entity
    {
        public virtual TestPointDao TestPoint { get; protected set; }

        public decimal Gauge { get; set; }
        public decimal? AtmosphericGauge { get; set; }
        public string ItemData { get; set; }
    }

    [Table("QA_TemperatureTests")]
    internal class TemperatureTestDao : Entity
    {
        public virtual TestPointDao TestPoint { get; protected set; }
        public decimal Gauge { get; set; }
        public string ItemData { get; set; }
    }

    [Table("QA_VolumeTests")]
    internal class VolumeTestDao : Entity
    {
        public virtual TestPointDao TestPoint { get; protected set; }

        public int PulserACount { get; set; }
        public int PulserBCount { get; set; }
        public int PulserCCount { get; set; }
        public decimal AppliedInput { get; set; }

        public string PreTestItemData { get; set; }
        public string PostTestItemData { get; set; }
    }
}
