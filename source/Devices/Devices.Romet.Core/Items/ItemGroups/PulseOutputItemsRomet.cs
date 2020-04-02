using System;
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
        private const int Channel_A_Type = 93;
        private const int Channel_A_Scaling = 115;

        private const int Channel_B_Type = 94;
        private const int Channel_B_Scaling = 115;

        private const int Channel_C_Type = 95;
        private const int Channel_C_Scaling = 115;

        private const int Unc_Channel_Units = 816;
        private const int Cor_Channel_Units = 817;

        protected override ChannelItems CreateChannel(ICollection<ItemValue> values, PulseOutputChannel channel,
            int? countItemNumber, int scalingItemNumber, int unitsItemNumber)
        {
            var channelType = (PulseOutputType) Enum.Parse(typeof(PulseOutputType), values.GetItem(unitsItemNumber).GetDescription());
            var channelUnits = GetChannelUnits(channelType, values);

            return new RometChannelItems
            {
                Name = channel,
                Count = countItemNumber.HasValue ? values.GetItemValueAsInt(countItemNumber.Value) : 0,
                Scaling = values.GetItemValueAsDecimal(scalingItemNumber),
                ChannelType = channelType,
                Multiplier = channelUnits.DecimalValue() ?? 0,
                Units = 
            };
        }

        private ItemValue GetChannelUnits(PulseOutputType channelType, ICollection<ItemValue> values)
        {
            switch (channelType)
            {
                case PulseOutputType.CorVol:
                    return values.GetItem(Cor_Channel_Units);
                case PulseOutputType.UncVol:
                    return values.GetItem(Unc_Channel_Units);
                default:
                    throw new NotImplementedException($"{channelType}");
            }
        }

        public override ItemGroup SetValues(DeviceType deviceType, IEnumerable<ItemValue> itemValues)
        {
            var values = itemValues.ToList();

            Channels = new List<ChannelItems>
            {
                CreateChannel(values, PulseOutputChannel.Channel_A, null, Channel_A_Scaling, Channel_A_Type),
                CreateChannel(values, PulseOutputChannel.Channel_B, null, Channel_B_Scaling, Channel_B_Type),
                //CreateChannel(values, PulseOutputChannel.Channel_C, null, Channel_C_Scaling, Channel_C_Units)
            };

            //base.SetValues(deviceType, values);

            return this;
        }

        public class RometChannelItems : ChannelItems
        {
            public decimal Multiplier { get; set; }
            public string Units { get; set; }
        }
    }
}
