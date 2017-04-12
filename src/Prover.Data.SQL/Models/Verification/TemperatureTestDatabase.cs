using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Prover.Data.EF.Models.TestPoint;

namespace Prover.Data.EF.Models
{
    [Table("QA_TemperatureTests")]
    internal class TemperatureTestDatabase
    {
        public Guid Id { get; set; }

        //[Required]
        //public Guid TestPointId { get; set; }
        //public virtual TestPointDao TestPoint { get; set; }

        public decimal GaugeTemperature { get; set; }
        public string ItemData { get; set; }
    }
}