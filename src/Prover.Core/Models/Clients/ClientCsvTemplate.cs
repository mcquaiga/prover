using System;
using System.ComponentModel.DataAnnotations;
using Prover.Core.Exports;
using Prover.Core.Models.Instruments;

namespace Prover.Core.Models.Clients
{
    public class ClientCsvTemplate
    {
        public ClientCsvTemplate()
        {
            Id = Guid.NewGuid();
        }

        public ClientCsvTemplate(Client client)
        {
            Client = client;
            ClientId = client.Id;
        }
        public Guid Id { get; set; }

        [Required]
        public virtual Client Client { get; set; }
        
        public Guid ClientId { get; set; }

        public VerificationTypEnum VerificationType { get; set; }
        public CorrectorType CorrectorType { get; set; }    
        public string CsvTemplate { get; set; }
    }
}
