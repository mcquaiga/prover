using System;
using Devices.Core.Interfaces;
using Prover.Application.Models.EvcVerifications.Builders;
using Prover.Application.Models.EvcVerifications.Verifications;
using Prover.Application.Models.EvcVerifications.Verifications.Volume.InputTypes;

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

        public EvcVerificationTest(DeviceInstance device)
        {
            Device = device;
            DriveType = VolumeInputTypes.Create(Device);
        }

        public DateTime? ArchivedDateTime { get; set; } = null;

        public DateTime TestDateTime { get; set; } = DateTime.Now;

        public DateTime? SubmittedDateTime { get; set; }

        public DateTime? ExportedDateTime { get; set; } = null;

        public DeviceInstance Device { get; protected set; }

        public IVolumeInputType DriveType { get; set; }

        public bool Verified { get; set; }

        public string JobId { get; set; } 

        public string EmployeeId { get; set; }
    }
}
