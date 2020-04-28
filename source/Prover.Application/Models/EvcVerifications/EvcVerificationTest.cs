using Devices.Core.Interfaces;
using Newtonsoft.Json;
using Prover.Application.Models.EvcVerifications.Builders;
using Prover.Application.Models.EvcVerifications.Verifications;
using Prover.Application.Models.EvcVerifications.Verifications.Volume.InputTypes;
using System;

namespace Prover.Application.Models.EvcVerifications
{
	/// <summary>
	///     Defines the <see cref="EvcVerificationTest" />
	/// </summary>
	public class EvcVerificationTest : AggregateRootWithChildTests<VerificationEntity>, IVerification
	{
		private EvcVerificationTest()
		{
		}

		[JsonConstructor]
		public EvcVerificationTest(DeviceInstance device)
		{
			Device = device;
			TestDateTime = DateTime.Now;
			DriveType = VolumeInputBuilderFactory.GetBuilder(Device).BuildVolumeType();
		}

		public DateTime? ArchivedDateTime { get; set; } = null;

		public DateTime TestDateTime { get; set; }

		//public DateTime TestDateTimeUtc { get; set; }

		public DateTime? SubmittedDateTime { get; set; }

		public DateTime? ExportedDateTime { get; set; } = null;

		public DeviceInstance Device { get; protected set; }

		[JsonIgnore] public IVolumeInputType DriveType { get; }

		public bool Verified { get; set; }

		public string JobId { get; set; }

		public string EmployeeId { get; set; }
	}
}