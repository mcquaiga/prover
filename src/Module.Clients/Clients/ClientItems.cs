using Core.Domain;
using Core.Extensions;
using Devices.Core.Items;
using System;
using System.Collections.Generic;

namespace Module.Clients.Clients
{
    public enum ClientItemType
    {
        Reset,

        Verify
    }

    public class ClientItems : BaseEntity
    {
        #region Constructors

        public ClientItems()
        {
        }

        public ClientItems(Client client)
        {
            Client = client ?? throw new NullReferenceException(nameof(client));
            ClientId = client.Id;
            Items = new List<ItemValue>();
        }

        #endregion

        #region Properties

        public Client Client { get; set; }

        public Guid ClientId { get; set; }

        public ClientItemType ItemFileType { get; set; }

        public string ItemFileTypeString
        {
            get => ItemFileType.ToString();
            private set => ItemFileType = value.ParseEnum<ClientItemType>();
        }

        public List<ItemValue> Items { get; private set; }

        #endregion
    }
}