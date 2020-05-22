using System;
using System.Collections.Generic;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Prover.Shared.Domain;

namespace Prover.Modules.Certificates.App.Models
{

	public enum ClientItemType
	{
		Reset,
		Verify
	}
	public class ClientDeviceItems : BaseEntity
	{
		public ClientDeviceItems()
		{
		}

		public ClientDeviceItems(Client client) => Client = client ?? throw new NullReferenceException(nameof(client));

		public ICollection<ItemValue> Items { get; set; } = new List<ItemValue>();

		public Client Client { get; set; }

		public ClientItemType ItemFileType { get; set; }

		public IDevice DeviceType { get; set; }
	}
}