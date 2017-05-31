using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.Exports;
using Prover.Core.Models.Instruments;

namespace Prover.Core.Models.Clients
{
    public class ClientCsvTemplatescs
    {
        public ClientCsvTemplatescs()
        {
            Id = Guid.NewGuid();
        }

        public ClientCsvTemplatescs(Client client)
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
