using Core.GasCalculations;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Domain.EvcVerifications.Verifications.CorrectionFactors;
using Domain.EvcVerifications.Verifications.Volume.InputTypes;
using Domain.EvcVerifications.Verifications.Volume.InputTypes.Rotary;
using System;
using System.Collections.Generic;

namespace Domain.EvcVerifications.Builders
{
    public static class VolumeInputTypes
    {
        private static readonly Dictionary<VolumeInputType, Func<IVolumeInputBuilder>> _volumeInputTypeBuilders =
            new Dictionary<VolumeInputType, Func<IVolumeInputBuilder>>
            {
                {VolumeInputType.Rotary, () => new RotaryVolumeInputBuilder()},
                {VolumeInputType.Mechanical, () => new MechanicalVolumeInputBuilder()},
                {VolumeInputType.PulseInput, () => new PulseInputVolumeBuilder()}
            };

        public static IVolumeInputType Create(DeviceInstance device)
        {
            return VolumeBuilder(device.ItemGroup<VolumeItems>().VolumeInputType).BuildVolumeInputType(device);
        }

        private static IVolumeInputBuilder VolumeBuilder(VolumeInputType volumeInputType)
        {
            return _volumeInputTypeBuilders[volumeInputType].Invoke();
        }
    }


    internal interface IVolumeInputBuilder
    {
        IVolumeInputType BuildVolumeInputType(DeviceInstance device);
    }

     internal class RotaryVolumeInputBuilder : IVolumeInputBuilder
    {
        #region Public Methods

        public IVolumeInputType BuildVolumeInputType(DeviceInstance device)
        {
            return new RotaryVolumeInputType(
                device.ItemGroup<VolumeItems>(), 
                device.ItemGroup<RotaryMeterItems>());
           
        }

        #endregion
    }

    internal class MechanicalVolumeInputBuilder : IVolumeInputBuilder
    {
        #region Public Methods

        public IVolumeInputType BuildVolumeInputType(DeviceInstance device)
        {
            return new MechanicalVolumeInputType(device.ItemGroup<VolumeItems>());
        }

        #endregion
    }

    internal class PulseInputVolumeBuilder : IVolumeInputBuilder
    {
        #region Public Methods

        public IVolumeInputType BuildVolumeInputType(DeviceInstance device)
        {
            return new PulseInputSensor(device);
        }


        #endregion
    }
}