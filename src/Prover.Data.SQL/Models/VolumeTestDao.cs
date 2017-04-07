using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prover.Data.SQL.Models
{
    [Table("QA_VolumeTests")]
    internal class VolumeTestDao
    {
        public Guid Id { get; set; }

        [Required]
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