using Devices.Core.Interfaces;
using Prover.Application.Models.EvcVerifications.Verifications.Volume.InputTypes;
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


        public override IVolumeInputType BuildVolumeType() => throw new NotImplementedException();

        /// <inheritdoc />
        protected override void SpecificDefaults()
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc />
        //public ICollection<VerificationEntity> CreateVolumeTests() => throw new NotImplementedException();
    }
}