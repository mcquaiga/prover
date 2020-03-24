using System.Collections.Generic;
using Prover.Application.Services;

namespace Prover.Application.Interfaces
{
    public interface ICompleteVolumeTest
    {
        bool IsTestComplete(ICollection<PulseChannel> pulseChannels);

        void Start();
    }
}