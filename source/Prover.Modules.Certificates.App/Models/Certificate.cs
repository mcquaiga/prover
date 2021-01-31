using System.Collections.Generic;
using Prover.Application.Models.EvcVerifications;
using Prover.Modules.Clients.Core;
using Prover.Shared;
using Prover.Shared.Domain;

namespace Prover.Modules.Certificates.Models {
	public class Certificate : AggregateRoot {
		public ICollection<EvcVerificationTest> EvcVerifications { get; set; }

		public string CreatedBy { get; set; }

		public VerificationType VerificationType { get; set; }

		//public string Apparatus { get; set; }

		public Application.Models.EvcVerifications.Prover Apparatus { get; set; }

		public long CerificateNumber { get; set; }

		public Owner Owner { get; set; }

		public Region Region { get; set; }
	}


}