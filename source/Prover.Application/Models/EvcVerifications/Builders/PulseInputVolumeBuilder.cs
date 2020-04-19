using System;
using Devices.Core.Interfaces;
using Prover.Application.Models.EvcVerifications.Verifications.Volume.InputTypes;

namespace Prover.Application.Models.EvcVerifications.Builders
{
    internal class PulseInputVolumeBuilder : VolumeInputTestBuilder
    {
        public PulseInputVolumeBuilder(DeviceInstance device) : base(device)
        {
        }

        #region Public Methods

        #endregion

        /// <inheritdoc />
        public override IVolumeInputType BuildVolumeType() => throw new NotImplementedException();

        /// <inheritdoc />
        public override VolumeInputTestBuilder AddDefaults(VerificationTestPoint current, bool withPulseOutputs = true) => throw new NotImplementedException();

        /// <inheritdoc />
        //public ICollection<VerificationEntity> CreateVolumeTests() => throw new NotImplementedException();
    }
}