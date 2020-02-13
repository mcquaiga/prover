namespace Devices.Core.Items.ItemGroups
{
    public interface IPulseOutputChannels
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