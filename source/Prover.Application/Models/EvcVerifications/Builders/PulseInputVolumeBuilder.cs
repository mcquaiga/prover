using Devices.Core.Interfaces;
using Prover.Application.Models.EvcVerifications.Verifications.Volume;
using System;

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
        protected override UncorrectedVolumeTestRun GetDriveSpecificUncorrectedTest() => throw new NotImplementedException();

        /// <inheritdoc />
        protected override void SpecificDefaults()
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc />
        //public ICollection<VerificationEntity> CreateVolumeTests() => throw new NotImplementedException();
    }
}