using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Prover.Core.Models.Instruments;

namespace Prover.Modules.Certificates.Models
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

        public virtual ICollection<CertificateInstrument> Instruments { get; set; }

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

        public static Certificate CreateCertificate(string testedBy, string verificationType,
            List<Instrument> instruments)
        {
            //var certificateStore = new CertificateStore(container);
            //var number = certificateStore.Query().DefaultIfEmpty().Max(x => x.Number) + 1;

            //var certificate = new Certificate
            //{
            //    CreatedDateTime = DateTime.Now,
            //    VerificationType = verificationType,
            //    TestedBy = testedBy,
            //    Number = number,
            //    Instruments = new Collection<Instrument>()
            //};

            //instruments.ForEach(i =>
            //{
            //    i.CertificateId = certificate.Id;
            //    i.Certificate = certificate;
            //    certificate.Instruments.Add(i);
            //});

            //certificateStore.Upsert(certificate);
            return new Certificate();
        }
    }
}