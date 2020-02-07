using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Core.Items;
using Devices.Honeywell.Comm.CommClients;
using Devices.Honeywell.Comm.Messaging.Requests;
using Devices.Honeywell.Core;
using Devices.Honeywell.Core.Items;
using Devices.Romet.Core;

namespace Devices.Romet.Comm
{
    public class RometClient : HoneywellClientBase<RometDeviceType, RometDeviceInstance>
    {
        public RometClient(ICommPort commPort, RometDeviceType deviceType) : base(commPort, deviceType)
        {
        }

        public override async Task<ItemValue> GetItemValue(ItemMetadata itemNumber)
        {
            var response = await ExecuteCommandAsync(Commands.ReadItem(itemNumber.Number));
            return ItemValue.Create(itemNumber, response.RawValue);
        }

        public override async Task<IEnumerable<ItemValue>> GetItemValuesAsync(IEnumerable<ItemMetadata> itemNumbers)
        {
            var results = new List<ItemValue>();

            foreach (var itemNumber in itemNumbers)
            {
                var value = await GetItemValue(itemNumber);

                if (value.Value.ToString().Trim() != "NA") 
                    results.Add(value);
            }

            return results;
        }
    }
}
