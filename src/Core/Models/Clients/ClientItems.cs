using System;
using System.Collections.Generic;
using Prover.Core.Models.Instruments;

namespace Prover.Core.Models.Clients
{
    public enum ClientItemType
    {
        Reset,
        Verify
    }

    public class ClientItems : ProverBaseEntity
    {
        public ClientItems()
        {
        }

        public ClientItems(Client client)
        {
            Client = client ?? throw new NullReferenceException(nameof(client));
            ClientId = client.Id;
            Items = new List<ItemValue>();
        }

        public Guid ClientId { get; set; }

        [Required]
        public Client Client { get; set; }

        [Column("ItemFileType")]
        public string ItemFileTypeString
        {
            get => ItemFileType.ToString();
            private set => ItemFileType = value.ParseEnum<ClientItemType>();
        }

        [NotMapped]
        public ClientItemType ItemFileType { get; set; }

        [Column("InstrumentType")]
        public string InstrumentTypeString
        {
            get => InstrumentType.Id.ToString();
            private set => InstrumentType = HoneywellInstrumentTypes.GetById(int.Parse(value));
        }
    }
}