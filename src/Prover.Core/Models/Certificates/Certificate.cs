using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Autofac;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;

namespace Prover.Core.Models.Certificates
{
    public class Certificate
    {
        public Certificate()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string VerificationType { get; set; }
        public string TestedBy { get; set; }
        public long Number { get; set; }

        public virtual ICollection<Instrument> Instruments { get; set; }

        [NotMapped]
        public string SealExpirationDate
        {
            get
            {
                var period = 10; //Re-Verification
                if (VerificationType == "Verification")
                    period = 12;

                return CreatedDateTime.AddYears(period).ToString("yyyy-MM-dd");
            }
        }

        [NotMapped]
        public int InstrumentCount
        {
            get { return Instruments.Count(); }
        }

        
    }
}