using System.Collections.Generic;

namespace Devices.Core.Items.ItemGroups
{
    // ReSharper disable InconsistentNaming
    public class PulseOutputItems : ItemGroup
    {
        public virtual int A_Count { get; set; }
        public virtual string A_Units { get; set; }
        public virtual decimal A_Scaling { get; set; }

        public virtual int B_Count { get; set; }
        public virtual string B_Units { get; set; }
        public virtual decimal B_Scaling { get; set; }

        public virtual string C_Units { get; set; }
        public virtual decimal C_Scaling { get; set; }

        public virtual ICollection<PulseOutputChannelItems> Channels { get; set; } = new List<PulseOutputChannelItems>();
    }

    public class PulseOutputChannelItems : ItemGroup
    {
        public virtual string Name { get; set; }
        public virtual int Count { get; set; }
        public virtual string Units { get; set; }
        public virtual decimal Scaling { get; set; }
    }
}