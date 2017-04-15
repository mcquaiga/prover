using System;
using System.Collections.Generic;
using Prover.Shared.Domain;
using Prover.Shared.DTO.Instrument;

namespace Prover.Shared.DTO.TestRuns
{
    public class TestRunDto : Entity
    {
        public InstrumentDto Instrument { get; set; }
        public DateTime TestDateTime { get; set; }
        public DateTime? ArchivedDateTime { get; set; }
        public DateTime? ExportedDateTime { get; set; }
        
        public ICollection<TestPointDto> TestPoints { get; set; }
        
        public TestRunDto(Guid id) : base(id)
        {
        }
    }
}