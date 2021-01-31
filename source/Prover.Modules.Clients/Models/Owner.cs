using Prover.Shared.Domain;

namespace Prover.Modules.Clients.Core {

	public class Owner : AggregateRoot {
		public string Name { get; set; }

		public Address Address { get; set; }
		//public ICollection<Address> Address { get; set; } = new List<Address>();

		public string RegistrationId { get; set; }
	}
}