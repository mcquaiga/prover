using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Honeywell.Core.Items.ItemGroups;
using Prover.Shared;

namespace Devices.Romet.Core.Items.ItemGroups
{
    public class PulseOutputItemsRomet : PulseOutputItemsHoneywell
    {
        private const int Channel_A_Units = 93;
        private const int Channel_A_Scaling = 115;

        private const int Channel_B_Units = 94;
        private const int Channel_B_Scaling = 115;

        private const int Channel_C_Units = 95;
        private const int Channel_C_Scaling = 115;

        public override ItemGroup SetValues(DeviceType deviceType, IEnumerable<ItemValue> itemValues)
        {
            var values = itemValues.ToList();

            Channels = new List<ChannelItems>
            {
                CreateChannel(values, PulseOutputChannel.Channel_A, null, Channel_A_Scaling, Channel_A_Units),
                CreateChannel(values, PulseOutputChannel.Channel_B, null, Channel_B_Scaling, Channel_B_Units),
                //CreateChannel(values, PulseOutputChannel.Channel_C, null, Channel_C_Scaling, Channel_C_Units)
            };

            //base.SetValues(deviceType, values);

            return this;
        }

    }
}
