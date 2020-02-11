using Devices.Core.Interfaces;

namespace Devices.Core.Items.ItemGroups
{
    public interface IPulseOutputChannels : IItemGroup
    {
        IPulseOutputs ChannelA { get; }
        IPulseOutputs ChannelB { get; }
        IPulseOutputs ChannelC { get; }

        decimal PulseOutputTiming { get; }
    }

    public interface IPulseOutputs
    {
        decimal Scaling { get; }
        PulseOutputUnitType Units { get; }
    }
}