using System;
using Prover.Shared.Domain;

namespace Prover.Application.Models.EvcVerifications {
	public class ProvingApparatus : EntityBase {
		public ProvingApparatus() { }

		public ProvingApparatus(Guid id) {
			Id = id;
		}

		public string DisplayName { get; set; }

		public string ApparatusDescriptor { get; set; }
	}


}