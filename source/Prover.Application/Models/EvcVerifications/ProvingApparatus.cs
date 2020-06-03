using System.Collections.Generic;
using Prover.Shared.Domain;

namespace Prover.Application.Models.EvcVerifications {
	public class ProverConfiguration : EntityBase {
		public ICollection<Apparatus> ProvingDevices { get; set; }

		//public int VersionNumber { get; set; }
	}


}