using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Prover.Data.EF.Models.PressureTest;
using Prover.Data.EF.Models.TestRun;

namespace Prover.Data.EF.Models.TestPoint
{
    [Table("QA_TestPoints")]
    internal class TestPointDatabase
    {
        public Guid Id { get; set; }
        public int Level { get; set; }

        [Required]
        public Guid TestRunId { get; set; }
        public virtual TestRunDatabase TestRun { get; set; }

        public Guid? PressureId => Pressure?.Id;
        public virtual PressureTestDatabase Pressure { get; set; }

        public Guid? TemperatureId => Temperature?.Id;
        public virtual TemperatureTestDatabase Temperature { get; set; }

        public Guid? VolumeId => Volume?.Id;
        public virtual VolumeTestDatabase Volume { get; set; }
    }
}