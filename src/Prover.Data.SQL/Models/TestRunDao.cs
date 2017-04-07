using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prover.Data.SQL.Models
{
    [Table("QA_TestRuns")]
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
}