using Prover.Shared.Domain;

namespace Prover.Modules.Clients.Core {
	public class Client : AggregateRoot {
		public string Name { get; set; }

		public Address Address { get; set; }

		public string RegistrationId { get; set; }
	}

	public class Address : EntityBase {

		public string Full { get; set; }

		public string Line1 { get; set; }
		public string Line2 { get; set; }
		public string Line3 { get; set; }
		public string Line4 { get; set; }

	}
}