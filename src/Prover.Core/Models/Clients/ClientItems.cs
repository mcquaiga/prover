using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;

namespace Prover.Core.Models.Clients
{
    public enum ItemType
    {
        PreTest,
        PostTest,
        Verify
    }

    public class ClientItems : ProverTable
    {
        public ClientItems()
        {
            
        }

        public ClientItems(Client client)
        {
            Client = client;
            ClientId = client.Id;
            Items = new List<ItemValue>();
        }

        public Guid ClientId { get; set; }

        [Required]
        public Client Client { get; set; }

        [Column("ItemFileType")]
        public string ItemFileTypeString
        {
            get { return ItemFileType.ToString(); }
            private set { ItemFileType = value.ParseEnum<ItemType>(); }
        }

        [NotMapped]
        public ItemType ItemFileType { get; set; }       

        [Column("InstrumentType")]
        public string InstrumentTypeString
        {
            get { return InstrumentType.Id.ToString(); }
            private set { InstrumentType = CommProtocol.MiHoneywell.Instruments.GetAll().FirstOrDefault(i => i.Id == int.Parse(value)); }            
        }
    }
}
