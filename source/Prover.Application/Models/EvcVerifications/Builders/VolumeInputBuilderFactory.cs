using System;
using System.Collections.Generic;
using Devices.Core.Interfaces;
using Prover.Shared;

namespace Prover.Application.Models.EvcVerifications.Builders
{
    public static class VolumeInputBuilderFactory
    {
        private static readonly Dictionary<VolumeInputType, Func<DeviceInstance, VolumeInputTestBuilder>> _volumeInputTypeBuilders =
                new Dictionary<VolumeInputType, Func<DeviceInstance, VolumeInputTestBuilder>>
                {
                        {VolumeInputType.Rotary, (device) => new RotaryVolumeInputBuilder(device)},
                        {VolumeInputType.Mechanical, (device) => new MechanicalVolumeInputBuilder(device)},
                        {VolumeInputType.PulseInput, (device) => new PulseInputVolumeBuilder(device)}
                };

        public static VolumeInputTestBuilder GetBuilder(DeviceInstance device)
        {
            return _volumeInputTypeBuilders[device.Volume()
                                                  .VolumeInputType]
                    .Invoke(device);
        }
    }
}