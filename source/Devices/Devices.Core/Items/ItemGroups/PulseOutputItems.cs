using System.Collections.Generic;
using System.Linq;
using Prover.Shared;

namespace Devices.Core.Items.ItemGroups
{
    // ReSharper disable InconsistentNaming
    public class PulseOutputItems : ItemGroup
    {
        public virtual ICollection<ChannelItems> Channels { get; set; } =
            new List<ChannelItems>();

        #region Nested type: ChannelItems

        public class ChannelItems : ItemGroup
        {
            public virtual PulseOutputChannel Name { get; set; }
            public virtual PulseOutputType ChannelType { get; set; }
            public virtual int Count { get; set; }
            public virtual decimal Scaling { get; set; }
        }

        #endregion
    }

    public static class PulseOutputItemEx
    {
        public static PulseOutputItems.ChannelItems ChannelByUnitType(this PulseOutputItems items, PulseOutputType type)
        {
            return items.Channels.FirstOrDefault(c => c.ChannelType == type);
        }

        public static PulseOutputItems.ChannelItems GetChannel(this PulseOutputItems items, PulseOutputChannel channel)
        {
            return items.Channels.FirstOrDefault(c => c.Name == channel);
        }

        public static PulseOutputItems.ChannelItems UncorrectedChannel(this PulseOutputItems items)
        {
            return items.Channels.FirstOrDefault(c => c.ChannelType == PulseOutputType.UncVol);
        }
        public static PulseOutputItems.ChannelItems CorrectedChannel(this PulseOutputItems items)
        {
            return items.Channels.FirstOrDefault(c => c.ChannelType == PulseOutputType.CorVol);
        }
    }
}