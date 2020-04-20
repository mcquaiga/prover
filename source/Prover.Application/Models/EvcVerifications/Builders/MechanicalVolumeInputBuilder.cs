using Devices.Core.Interfaces;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Models.EvcVerifications.Verifications.Volume;
using Prover.Application.Models.EvcVerifications.Verifications.Volume.InputTypes;

namespace Prover.Application.Models.EvcVerifications.Builders
{
    internal class MechanicalVolumeInputBuilder : VolumeInputTestBuilder
    {
        public MechanicalVolumeInputBuilder(DeviceInstance device) : base(device)
        {
        }

        /// <inheritdoc />
        public override VolumeInputTestBuilder AddDefaults(VerificationTestPoint current, bool withPulseOutputs = true)
        {
            AddUncorrected(withPulseOutputs);
            AddCorrected(withPulseOutputs);
            
            var energyTest = 
                    new EnergyTest(Device.CreateItemGroup<EnergyItems>(), Device.CreateItemGroup<EnergyItems>(), current.GetTest<CorrectedVolumeTestRun>().ActualValue);
            
            return this;
        }

        /// <inheritdoc />
        public override IVolumeInputType BuildVolumeType()
        {
            return new MechanicalVolumeInputType(Device.ItemGroup<VolumeItems>());
        }
    }
}