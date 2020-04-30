using Core.Domain;
using Devices.Core.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Module.Clients.Models
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

        public ClientItems(Client client, ICollection<ItemValue> items)
        {
            Client = client ??
                throw new NullReferenceException(nameof(client));

            Items = items;
        }

        #endregion

        #region Properties

        public Client Client { get; set; }

        public ClientItemType ItemFileType { get; set; }

        public ICollection<ItemValue> Items { get; } = new Collection<ItemValue>();

        #endregion
    }
}