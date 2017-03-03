using System.ComponentModel.DataAnnotations.Schema;
using Prover.Core.DAL.Common;
using Prover.Core.Domain.Enums;
using Prover.Core.Domain.Models.QaTestRuns.QaTestPoints;

namespace Prover.Core.DAL.DataAccess.QaTestRuns
{
    [Table("qa_temperature")]
    public class TemperatureDal : Entity
    {
        public virtual TestPointDal TestPoint { get; protected set; }

        public TemperatureUnits Units { get; set; }
        public decimal EvcTemperature { get; set; }
        public decimal EvcFactor { get; set; }
        public decimal EvcBaseTemperature { get; set; }

        public decimal GaugeTemperature { get; set; }
        public decimal ActualFactor { get; set; }
    }
}