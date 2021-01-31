using Prover.Shared.Domain;
using Prover.Shared.Interfaces;

namespace Prover.Modules.UnionGas.Models {
	public class Employee : EntityBase, IUser {
		/// <inheritdoc />
		public string UserId { get; set; }

		/// <inheritdoc />
		public string UserName { get; set; }

		public string EmployeeNumber { get; set; }
	}
}
