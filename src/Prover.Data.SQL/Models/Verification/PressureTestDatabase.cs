using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper;
using Prover.Data.EF.Mappers.Resolvers;
using Prover.Data.EF.Models.TestPoint;
using Prover.Data.EF.Models.TestRun;
using Prover.Shared.DTO.TestRuns;

namespace Prover.Data.EF.Models.PressureTest
{

    [Table("QA_PressureTests")]
    internal class PressureTestDatabase
    {
        [Key]
        public Guid Id { get; set; }

        //[Required]
        //public Guid TestPointId { get; set; }
        //public virtual TestPointDao TestPoint { get; set; }

        public decimal GaugePressure { get; set; }
        public decimal? AtmosphericGauge { get; set; }
        public string ItemData { get; set; }
    }
}