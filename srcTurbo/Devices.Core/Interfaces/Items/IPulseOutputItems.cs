namespace Devices.Core.Interfaces.Items
{
    public interface IPulseOutputChannels : IItemsGroup
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