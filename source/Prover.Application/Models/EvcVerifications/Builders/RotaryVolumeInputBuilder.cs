using System.Collections.Generic;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Models.EvcVerifications.Verifications.Volume.InputTypes;
using Prover.Application.Models.EvcVerifications.Verifications.Volume.InputTypes.Rotary;

namespace Prover.Application.Models.EvcVerifications.Builders
{
    internal class RotaryVolumeInputBuilder : VolumeInputTestBuilder
    {

        private readonly RotaryVolumeInputType _inputType;


        public RotaryVolumeInputBuilder(DeviceInstance device) : base(device)
        {
            _inputType = new RotaryVolumeInputType(Device.ItemGroup<VolumeItems>(), Device.ItemGroup<RotaryMeterItems>());
        }

        /// <inheritdoc />
        public override IVolumeInputType BuildVolumeType()
        {
            return _inputType;
        }

        public override VolumeInputTestBuilder AddDefaults(VerificationTestPoint current, bool withPulseOutputs = true)
        {
            AddUncorrected(withPulseOutputs);
            AddCorrected(withPulseOutputs);

            var rotaryTest = new RotaryMeterTest(Device.ItemGroup<RotaryMeterItems>());
            Tests.Add(rotaryTest);
            
            return this;
        }
    }
}