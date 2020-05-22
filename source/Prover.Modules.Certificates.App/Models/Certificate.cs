using System.Collections.Generic;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Models.EvcVerifications.Verifications;
using Prover.Shared.Domain;

namespace Prover.Modules.Certificates.App.Models
{
	public enum VerificationType
	{
		New,
		Reverified
	}

	public class Certificate : AggregateRoot
	{
		public ICollection<EvcVerificationTest> EvcVerifications { get; set; }

		public string CreatedBy { get; set; }

		public VerificationType VerificationType { get; set; }

		public string Apparatus { get; set; }

		public long CerificateNumber { get; set; }
	}


}