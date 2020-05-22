using Prover.Shared.Domain;

namespace Prover.Modules.Certificates.App.Models
{
	public class Client : AggregateRoot
	{
		public string Name { get; set; }

		public string Address { get; set; }

		public string RegistrationNumber { get; set; }
	}
}