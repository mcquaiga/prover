using System;
using System.Collections.Generic;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Domain.EvcVerifications.Builders;

namespace Domain.EvcVerifications.Verifications.Volume.InputTypes
{
    public interface IVolumeInputType
    {
        #region Public Properties

        VolumeInputType InputType { get; }

        #endregion

        #region Public Methods

        int MaxUncorrectedPulses();

        decimal UnCorrectedInputVolume(decimal appliedInput);

        #endregion
    }

    public static class VolumeInputTypes
    {
        private static readonly Dictionary<VolumeInputType, Func<VolumeInputBuilder>> _volumeInputTypeBuilders =
            new Dictionary<VolumeInputType, Func<VolumeInputBuilder>>
            {
                {VolumeInputType.Rotary, () => new RotaryVolumeInputBuilder()},
                {VolumeInputType.Mechanical, () => new MechanicalVolumeInputBuilder()},
                {VolumeInputType.PulseInput, () => new PulseInputVolumeBuilder()}
            };

        public static IVolumeInputType Create(DeviceInstance device,
            EvcVerificationTest evcVerification)
        {
            var i = VolumeBuilder(device.ItemGroup<VolumeItems>().VolumeInputType).BuildVolumeInputType(device, evcVerification);
            return evcVerification.DriveType;
        }

        public static IVolumeInputType Create(DeviceInstance device)
        {
            var test = new EvcVerificationTest(device);
            return Create(device, test);
        }


        private static VolumeInputBuilder VolumeBuilder(VolumeInputType volumeInputType)
        {
            return _volumeInputTypeBuilders[volumeInputType].Invoke();
        }
    }
}