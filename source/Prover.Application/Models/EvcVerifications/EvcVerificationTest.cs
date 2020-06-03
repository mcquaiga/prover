using System;
using System.Collections.Generic;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Devices.Core.Interfaces;
using Newtonsoft.Json;
using Prover.Application.Models.EvcVerifications.Verifications;
using Prover.Shared.Domain;

namespace Prover.Application.Models.EvcVerifications {
	/// <summary>
	///     Defines the <see cref="EvcVerificationTest" />
	/// </summary>
	public class EvcVerificationTest : AggregateRoot<VerificationEntity>, IVerification {
		private EvcVerificationTest() { }

		[JsonConstructor]
		public EvcVerificationTest(DeviceInstance device) {
			Device = device;
		}

		public DateTime TestDateTime { get; set; } = DateTime.Now;

		public DateTime? SubmittedDateTime { get; set; }

		public DateTime? ExportedDateTime { get; set; } = null;

		public ProverConfiguration ProverConfiguration { get; set; }

		public DeviceInstance Device { get; protected set; }

		public ICollection<VerificationEntity> Tests { get; set; } = new List<VerificationEntity>();

		[Ignore]
		public VerificationBase Verification { get; set; }

		public bool Verified { get; set; }

		public string JobId { get; set; }

		public string EmployeeId { get; set; }

		/// <inheritdoc />
		protected override ICollection<VerificationEntity> Children => Tests;
	}
}