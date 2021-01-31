using System;
using System.Collections.Generic;
using Prover.Shared.Domain;

namespace Prover.Application.Models.EvcVerifications {
	public class Prover : AggregateRoot {
		public Prover() { }

		public Prover(Guid id) {
			Id = id;
		}

		public string DisplayName { get; set; }

		public string Descriptor { get; set; }

		public ICollection<ProverConfiguration> ConfigurationHistory { get; set; }
	}

	//public class MachineProver : EntityBase {
	//	public string ComputerName { get; set; }
	//	public Prover Prover { get; set; }
	//}
}