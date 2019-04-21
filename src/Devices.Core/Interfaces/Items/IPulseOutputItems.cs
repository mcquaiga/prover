namespace Devices.Core.Interfaces.Items
{
    public interface IPulseOutputChannels : IItemsGroup
    {
        #region Public Properties

        IPulseOutputs ChannelA { get; }
        IPulseOutputs ChannelB { get; }
        IPulseOutputs ChannelC { get; }
        decimal PulseOutputTiming { get; }

        #endregion Public Properties
    }

    public interface IPulseOutputs : IItemsGroup
    {
        #region Public Properties

        decimal Scaling { get; }
        decimal Units { get; }

        #endregion Public Properties
    }
}