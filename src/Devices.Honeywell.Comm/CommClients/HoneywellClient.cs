using Devices.Communications;
using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using Devices.Honeywell.Comm.Messaging.Requests;
using Devices.Honeywell.Comm.Messaging.Responses.Codes;
using Devices.Honeywell.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Devices.Honeywell.Comm.CommClients
{
    public class HoneywellClient : EvcCommunicationClient<IHoneywellDeviceType>
    {
        internal HoneywellClient(ICommPort commPort) : base(commPort)
        {
        }

        public override bool IsConnected { get; protected set; }
        protected Task LoadItemsTask { get; }

        public override async Task Disconnect()
        {
            if (IsConnected)
            {
                var response = await ExecuteCommandAsync(Commands.SignOffCommand());

                if (response.IsSuccess)
                    IsConnected = false;
            }
        }

        public override Task<IFrequencyTestItems> GetFrequencyItems()
        {
            throw new NotImplementedException();
        }

        public override async Task<ItemValue> GetItemValue(ItemMetadata itemNumber)
        {
            var itemDetails = itemNumber;
            var response = await ExecuteCommandAsync(Commands.ReadItem(itemNumber.Number));
            return new ItemValue(itemDetails, response.RawValue);
        }

        public override async Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<ItemMetadata> itemNumbers)
        {
            var itemDetails = itemNumbers.ToList();
            var items = itemDetails.GetAllItemNumbers().ToArray();
            var results = new List<ItemValue>();

            if (items.Count() == 1)
            {
                var value = await GetItemValue(itemDetails[0]);
                results.Add(value);
                return results;
            }

            items = items.OrderBy(x => x).ToArray();

            var y = 0;

            while (true)
            {
                var set = items.Skip(y).Take(15).ToList();

                if (!set.Any())
                    break;

                var response = await ExecuteCommandAsync(Commands.ReadGroup(set));
                foreach (var item in response.ItemValues)
                {
                    var metadata = itemDetails.FirstOrDefault(x => x.Number == item.Key);
                    results.Add(new ItemValue(metadata, item.Value));
                }

                y += 15;
            }

            return results;
        }

        public override async Task<ItemValue> LiveReadItemValue(int itemNumber)
        {
            var itemDetails = ItemDetails.GetItem(itemNumber);
            var response = await ExecuteCommandAsync(Commands.LiveReadItem(itemNumber));
            return new ItemValue(itemDetails, response.RawValue);
        }

        public override async Task<bool> SetItemValue(int itemNumber, string value)
        {
            var result = await ExecuteCommandAsync(Commands.WriteItem(itemNumber, value));

            return result.IsSuccess;
        }

        public override async Task<bool> SetItemValue(int itemNumber, decimal value)
            => await SetItemValue(itemNumber, value.ToString(CultureInfo.InvariantCulture));

        public override async Task<bool> SetItemValue(int itemNumber, long value)
            => await SetItemValue(itemNumber, value.ToString());

        public override async Task<bool> SetItemValue(string itemCode, long value)
            => await SetItemValue(ItemDetails.GetItem(itemCode), value);

        internal new Task ConnectAsync(IDeviceType deviceType, int retryAttempts = 10, TimeSpan? timeout = null)
        {
            return base.ConnectAsync(deviceType, retryAttempts, timeout);
        }

        protected override async Task EstablishConnectionAsync(IDeviceType deviceType, CancellationToken ct)
        {
            if (await WakeUpInstrument(ct))
            {
                var response = await ExecuteCommandAsync(Commands.SignOn((IHoneywellDeviceType)deviceType));

                if (response.IsSuccess)
                {
                    IsConnected = true;
                    StatusStream.OnNext($"[{CommPort.Name}] Connected to {deviceType.Name}!");
                }
                else
                {
                    IsConnected = false;

                    var responseType = Responses.Get(response.ResponseCode);

                    responseType.TryRecover(this);

                    if (responseType.ThrowsException)
                        throw responseType.RaiseException(response);
                }
            }
        }

        private async Task<bool> WakeUpInstrument(CancellationToken ct)
        {
            await ExecuteCommandAsync(Commands.WakeupOne());
            await Task.Delay(250);

            try
            {
                var response = await ExecuteCommandAsync(Commands.WakeupTwo());

                if (response.IsSuccess || (response.ResponseCode == ResponseCode.InvalidEnquiryError))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                await Task.Delay(200);
                await ExecuteCommandAsync(Commands.OkayToSend());
                throw;
            }

            return false;
        }
    }
}