using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Models.EvcVerifications.Verifications;
using Prover.Application.Models.EvcVerifications.Verifications.Volume;
using Prover.Application.Models.EvcVerifications.Verifications.Volume.InputTypes;
using Prover.Shared;

namespace Prover.Application.Models.EvcVerifications.Builders
{
    public abstract class VolumeInputTestBuilder
    {
        private VolumeItems _endItems;
        private VolumeItems _startItems;
        private int _appliedInput;
        private int _uncorPulses;
        private int _corPulses;

        protected VolumeInputTestBuilder(DeviceInstance device)
        {
            Device = device;
            _startItems = Device.CreateItemGroup<VolumeItems>();
            _endItems = Device.CreateItemGroup<VolumeItems>();
        }

        protected DeviceInstance Device { get; }
        protected List<VerificationEntity> Tests { get; } = new List<VerificationEntity>();


        public virtual VolumeInputTestBuilder AddCorrected(bool withPulseOutputs = true)
        {
            var corrected = new CorrectedVolumeTestRun(_startItems, _endItems, 0m, 0m, 100m, false, 0m, _appliedInput);
            if (withPulseOutputs) corrected.PulseOutputTest = WithPulseOutput(PulseOutputType.CorVol, _corPulses);
            Tests.Add(corrected);
            return this;
        }
        
        public abstract VolumeInputTestBuilder AddDefaults(VerificationTestPoint currentool, bool withPulseOutputs = true);

        public virtual VolumeInputTestBuilder AddUncorrected(bool withPulseOutputs = true)
        {
            var uncorrected = new UncorrectedVolumeTestRun(_startItems, _endItems, 0m, 0m, 100m, false, _appliedInput);

            if (withPulseOutputs)
                uncorrected.PulseOutputTest = WithPulseOutput(PulseOutputType.UncVol, _uncorPulses);

            Tests.Add(uncorrected);
            return this;
        }

        public ICollection<VerificationEntity> Build() => Tests;

        /// <inheritdoc />
        public abstract IVolumeInputType BuildVolumeType();

 
        
        protected virtual PulseOutputVerification WithPulseOutput(PulseOutputType pulseType, int pulseCount)
        {
            var items = Device.ItemGroup<PulseOutputItems>()
                              .Channels.FirstOrDefault(c => c.ChannelType == pulseType);
            return items != null 
                    ? new PulseOutputVerification(items, 0m, 
                            pulseCount, 100m) : default;
        }


        public virtual void SetItemValues(ICollection<ItemValue> startValues, ICollection<ItemValue> endValues, int? appliedInput = null, int? corPulses = null, int? uncorPulses = null)
        {
            _startItems = Device.CreateItemGroup<VolumeItems>(startValues) ?? _startItems;
            _endItems = Device.CreateItemGroup<VolumeItems>(endValues) ?? _endItems;

            _appliedInput = appliedInput ?? 0;
            _uncorPulses = uncorPulses ?? 0;
            _corPulses = corPulses ?? 0;
        }
    }
}