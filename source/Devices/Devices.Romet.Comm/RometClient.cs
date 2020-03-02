using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devices.Communications.Status;
using Devices.Core.Items;
using Devices.Honeywell.Comm.CommClients;
using Devices.Honeywell.Comm.Messaging.Requests;
using Devices.Romet.Core;
using Prover.Shared.IO;

namespace Devices.Romet.Comm
{
    public class RometClient : HoneywellClientBase<RometDeviceType>
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
            var itemDetails = itemNumbers.ToList();
            foreach (var itemNumber in itemDetails)
            {
                var value = await GetItemValue(itemNumber);

                if (value.GetValue().ToString().Trim() != "NA") 
                    results.Add(value);

                this.MessageItemReadStatus(itemDetails, results);
            }

            return results;
        }

        public override async Task<ItemValue> LiveReadItemValue(int itemNumber)
        {
            var itemDetails = DeviceType.Items.GetItem(itemNumber);
            if (itemDetails == null)
                throw new Exception($"Item with #{itemNumber} does not exist in metadata.");
            
            return await GetItemValue(itemDetails);
        }
    }
}
