using System;
using Devices.Core.Interfaces;
using Prover.Domain.EvcVerifications.Builders;
using Prover.Domain.EvcVerifications.Verifications;
using Prover.Domain.EvcVerifications.Verifications.Volume.InputTypes;

namespace Prover.Domain.EvcVerifications
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

        public DateTime? ExportedDateTime { get; set; } = null;

        public DeviceInstance Device { get; protected set; }

        public IVolumeInputType DriveType { get; set; }

        public bool Verified { get; set; }
    }
}
