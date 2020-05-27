using System.Collections.Generic;
using Prover.Shared.Domain;

namespace Prover.Modules.Clients.Core {

	public class Owner : AggregateRoot {
		public string Name { get; set; }

		public ICollection<Address> Address { get; set; } = new List<Address>();

		public string RegistrationId { get; set; }
	}

	public class Region : EntityBase {
		public string Code { get; set; }
		public string Description { get; set; }

		public Region ParentRegion { get; set; }
		public ICollection<Region> ChildRegions { get; set; } = new List<Region>();
	}
}