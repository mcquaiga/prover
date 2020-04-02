using System.Collections.Generic;
using System.Linq;
using Prover.Application.Interfaces;
using Prover.Domain.EvcVerifications.Verifications.Volume.InputTypes.Rotary;
using Prover.Shared;

namespace Prover.Application.Services
{
    public class RotaryVolumeTestTerminator : ICompleteVolumeTest
    {
        private readonly RotaryVolumeInputType _rotaryInputType;

        public RotaryVolumeTestTerminator(RotaryVolumeInputType rotaryInputType) => _rotaryInputType = rotaryInputType;

        public bool IsTestComplete(ICollection<PulseChannel> pulseChannels)
        {
            var uncChannel = pulseChannels.FirstOrDefault(p => p.Items.ChannelType == PulseOutputType.UncVol);

            if (MaxUncorrectedPulses() == uncChannel?.PulseCount) return true;

            return false;
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        private int MaxUncorrectedPulses()
        {
            if (_rotaryInputType.VolumeItems.UncorrectedMultiplier == 10)
                return _rotaryInputType.RotaryItems.MeterType.UnCorPulsesX10;

            if (_rotaryInputType.VolumeItems.UncorrectedMultiplier == 100)
                return _rotaryInputType.RotaryItems.MeterType.UnCorPulsesX100;

            return 10; //Low standard number if we can't find anything
        }
    }
}