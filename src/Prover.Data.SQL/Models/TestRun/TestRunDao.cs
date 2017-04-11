using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Prover.Data.EF.Models.TestPoint;

namespace Prover.Data.EF.Models.TestRun
{
    [Table("QA_TestRuns")]
    internal class TestRunDao
    {
        [Key]
        public Guid Id { get; set; }
        
        public string Instrument { get; set; }

        public DateTime TestDateTime { get; set; }
        public DateTime? ExportedDateTime { get; set; }
        public DateTime? ArchivedDateTime { get; set; }

        public string ItemData { get; set; }

        public virtual ICollection<TestPointDao> TestPoints { get; set; }
    }
}