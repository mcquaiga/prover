namespace Prover.CommProtocol.Common.Models.Instrument.Items
{
    public interface IPulseOutputs : IItemsGroup
    {
        decimal Scaling { get; }
        decimal Units { get; }
    }

    public interface IPulseOutputChannels : IItemsGroup
    {
        IPulseOutputs ChannelA { get; }
        IPulseOutputs ChannelB { get; }       
        IPulseOutputs ChannelC { get; }
        decimal PulseOutputTiming { get; }
    }
}