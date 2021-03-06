using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Prover.Shared;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public class PulseOutputItemsHoneywell : PulseOutputItems
    {
        private const int Channel_A_Count = 5;
        private const int Channel_A_Units = 93;
        private const int Channel_A_Scaling = 56;

        private const int Channel_B_Count = 6;
        private const int Channel_B_Units = 94;
        private const int Channel_B_Scaling = 57;

        private const int Channel_C_Units = 95;
        private const int Channel_C_Scaling = 58;


        public override ItemGroup SetValues(DeviceType deviceType, IEnumerable<ItemValue> itemValues)
        {
            var values = itemValues.ToList();

            Channels = new List<ChannelItems>
            {
                CreateChannel(values, PulseOutputChannel.Channel_A, Channel_A_Count, Channel_A_Scaling,
                    Channel_A_Units),
                CreateChannel(values, PulseOutputChannel.Channel_B, Channel_B_Count, Channel_B_Scaling,
                    Channel_B_Units),
                //CreateChannel(values, PulseOutputChannel.Channel_C, null, Channel_C_Scaling, Channel_C_Units)
            };

            base.SetValues(deviceType, values);

            return this;
        }


        protected virtual ChannelItems CreateChannel(ICollection<ItemValue> values, PulseOutputChannel channel,
            int? countItemNumber, int scalingItemNumber, int unitsItemNumber)
        {
            return new PulseOutputChannelHoneywell
            {
                Name = channel,
                Count = countItemNumber.HasValue ? values.GetItemValueAsInt(countItemNumber.Value) : 0,
                Scaling = values.GetItemValueAsDecimal(scalingItemNumber),
                ChannelType = (PulseOutputType) Enum.Parse(typeof(PulseOutputType),
                    values.GetItem(unitsItemNumber).GetDescription())
            };
        }

        #region Nested type: PulseOutputChannelHoneywell

        public class PulseOutputChannelHoneywell : ChannelItems
        {
            public override PulseOutputChannel Name { get; set; }
            public override int Count { get; set; }
            public override decimal Scaling { get; set; }
            public override PulseOutputType ChannelType { get; set; }
        }

        #endregion
    }

 
}