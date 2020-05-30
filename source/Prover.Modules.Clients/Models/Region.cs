using System.Collections.Generic;
using Prover.Shared.Domain;

namespace Prover.Modules.Clients.Core {
	public class Region : AggregateRoot {
		public string Code { get; set; }
		public string Description { get; set; }

		public Region ParentRegion { get; set; }
		public ICollection<Region> ChildRegions { get; set; } = new List<Region>();
	}
}