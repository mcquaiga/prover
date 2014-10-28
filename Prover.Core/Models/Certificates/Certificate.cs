using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
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
            get {  return CreatedDateTime.AddYears(5).ToString("yyyy-MM-dd"); }
        }
        
        [NotMapped]
        public int InstrumentCount 
        {
            get { return Instruments.Count(); }
        }

        public static Certificate CreateCertificate(IUnityContainer container, string testedBy, string verificationType, List<Instrument> instruments)
        {

            var certificateStore = new CertificateStore(container);
            var number = certificateStore.Query().Max(x => x.Number) + 1;

            var certificate = new Certificate
            {
                CreatedDateTime = DateTime.Now,
                VerificationType = verificationType,
                TestedBy = testedBy,
                Number = number,
                Instruments = new Collection<Instrument>()
            };


            instruments.ForEach(i =>
            {
                i.CertificateId = certificate.Id;
                i.Certificate = certificate;
                certificate.Instruments.Add(i);

            });

            

            certificateStore.Upsert(certificate);
            return certificate;
        }
    }
}
