using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Prover.Application.Models.EvcVerifications.Verifications;
using Prover.Shared.Domain;

namespace Prover.Application.Models.EvcVerifications {
	public class VerificationBase : AggregateRoot<VerificationEntity>, IVerification {
		public VerificationBase() { }

		public VerificationBase(DeviceInstance device) {
			Device = device;
			//SetupChildCollection(Tests);
		}

		public DeviceInstance Device { get; protected set; }

		public ICollection<VerificationEntity> Tests { get; set; } = new List<VerificationEntity>();

		public bool Verified { get; set; }

		public ICollection<VerificationEntity> GetTests() => Tests;

		public ICollection<T> GetTests<T>() where T : VerificationEntity => (ICollection<T>)Tests.OfType<T>();

		/// <inheritdoc />
		protected override ICollection<VerificationEntity> Children => Tests;
	}
}