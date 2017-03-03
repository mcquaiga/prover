using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.DAL.Common;
using Prover.Core.Domain.Enums;
using Prover.Core.Domain.Models.QaTestRuns.QaTestPoints;

namespace Prover.Core.DAL.DataAccess.QaTestRuns
{
    [Table("qa_testpoints")]
    public class TestPointDal : Entity
    {
        [Required]
        public Guid QaTestRunId { get; protected set; }
        public virtual QaTestRunDal QaTestRun { get; protected set; }

        [NotMapped]
        public TestLevel Level { get; protected set; }

        public Guid? PressureId { get; protected set; }
        public virtual PressureDal Pressure { get; protected set; }

        public Guid? TemperatureId { get; protected set; }
        public virtual TemperatureDal Temperature { get; protected set; }

        public Guid? VolumeId { get; protected set; }
        public virtual VolumeDal Volume { get; protected set; }
    }
}
