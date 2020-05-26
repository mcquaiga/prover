using Devices.Core.Items;
using System;
using System.Collections.Generic;
using Devices.Core.Interfaces;
using Prover.Application.ValidationRules;
using Prover.Shared.Domain;

namespace Prover.Modules.Clients.Core {

	public enum ClientItemType {
		Reset,
		Verify
	}
	public class ClientValidationRules : EntityBase {
		public ClientValidationRules() {
		}


		public ClientValidationRules(Client client) => Client = client ?? throw new NullReferenceException(nameof(client));

		public ICollection<ItemValue> Items { get; set; } = new List<ItemValue>();

		public Client Client { get; set; }

		public ClientItemType ItemFileType { get; set; }

		public IDevice DeviceType { get; set; }

		public ICollection<ItemValidationRule> Rules { get; set; }
	}
}