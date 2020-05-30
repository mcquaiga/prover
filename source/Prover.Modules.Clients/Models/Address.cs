using Prover.Shared.Domain;

namespace Prover.Modules.Clients.Core {
	public class Address : EntityBase {

		public string Full { get; set; }
		public bool Default { get; set; }
		public string Province { get; set; }

		public string Line1 { get; set; }
		public string Line2 { get; set; }
		public string Line3 { get; set; }
		public string Line4 { get; set; }

	}
}