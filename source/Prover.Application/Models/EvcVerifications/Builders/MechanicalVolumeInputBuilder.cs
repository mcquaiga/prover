using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Models.EvcVerifications.Verifications.Volume;
using Prover.Application.Models.EvcVerifications.Verifications.Volume.InputTypes;

namespace Prover.Application.Models.EvcVerifications.Builders
{
    internal class MechanicalVolumeInputBuilder : VolumeInputTestBuilder
    {
        private EnergyItems _startEnergyItems;
        private EnergyItems _endEnergyItems;

        public MechanicalVolumeInputBuilder(DeviceInstance device) : base(device)
        {
        }

      
        /// <inheritdoc />
        public override VolumeInputTestBuilder AddDefaults(VerificationTestPoint current, bool withPulseOutputs = true)
        {
            AddUncorrected(withPulseOutputs);

            AddCorrected(withPulseOutputs);
            
            var energyTest = 
                    new EnergyTest(_startEnergyItems, _endEnergyItems, Tests.OfType<CorrectedVolumeTestRun>().First().ActualValue);
            Tests.Add(energyTest);
            return this;
        }

        /// <inheritdoc />
        public override IVolumeInputType BuildVolumeType()
        {
            return new MechanicalVolumeInputType(Device.ItemGroup<VolumeItems>());
        }

        /// <inheritdoc />
        public override void SetItemValues(ICollection<ItemValue> startValues, ICollection<ItemValue> endValues, int? appliedInput = null, int? corPulses = null, int? uncorPulses = null)
        {
            base.SetItemValues(startValues,
            endValues,
            appliedInput,
            corPulses,
            uncorPulses);

            _startEnergyItems = Device.CreateItemGroup<EnergyItems>(startValues);
            _endEnergyItems = Device.CreateItemGroup<EnergyItems>(endValues);
        }
    }
}