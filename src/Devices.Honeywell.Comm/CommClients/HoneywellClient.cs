using Devices.Communications;
using Devices.Communications.IO;
using Devices.Core.Items;
using Devices.Honeywell.Comm.Messaging.Requests;
using Devices.Honeywell.Comm.Messaging.Responses.Codes;
using Devices.Honeywell.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Devices.Honeywell.Comm.CommClients
{
    internal abstract class BaseHoneywellClient : CommunicationsClient
    {
        public override bool IsConnected { get; protected set; }

        public override async Task Disconnect()
        {
            if (IsConnected)
            {
                var response = await ExecuteCommandAsync(Commands.SignOffCommand());

                if (response.IsSuccess)
                    IsConnected = false;
            }
        }

        public async Task<ItemValue> GetItemValue(ItemMetadata itemNumber)
        {
            var itemDetails = itemNumber;
            var response = await ExecuteCommandAsync(Commands.ReadItem(itemNumber.Number));
            return new ItemValue(itemDetails, response.RawValue);
        }

        internal BaseHoneywellClient(ICommPort commPort, HoneywellDeviceType deviceType) : base(commPort, deviceType)
        {
        }

        protected Task LoadItemsTask { get; }
        //public override async Task<bool> SetItemValue(string itemCode, long value)
        //    => await SetItemValue(ItemDetails.GetItem(itemCode), value);

        protected override async Task EstablishConnectionAsync<T>(T deviceType, CancellationToken ct)
        {
            var honeyDevice = deviceType as HoneywellDeviceType;
            if (honeyDevice == null)
                throw new ArgumentException($"Type {typeof(T)} is not valid as a Honeywell Device Type.");

            if (await WakeUpInstrument(ct))
            {
                var response = await ExecuteCommandAsync(Commands.SignOn(honeyDevice));

                if (response.IsSuccess)
                {
                    IsConnected = true;
                    StatusStream.OnNext($"[{CommPort.Name}] Connected to {honeyDevice.Name}!");
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

        protected override async Task<IEnumerable<ItemValue>> GetItemValuesAsync(IEnumerable<ItemMetadata> itemNumbers)
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

        //public override async Task<ItemValue> LiveReadItemValue(int itemNumber)
        //{
        //    var itemDetails = ItemDetails.GetItem(itemNumber);
        //    var response = await ExecuteCommandAsync(Commands.LiveReadItem(itemNumber));
        //    return new ItemValue(itemDetails, response.RawValue);
        //}

        //public override async Task<bool> SetItemValue(int itemNumber, string value)
        //{
        //    var result = await ExecuteCommandAsync(Commands.WriteItem(itemNumber, value));

        //    return result.IsSuccess;
        //}

        //public override async Task<bool> SetItemValue(int itemNumber, decimal value)
        //    => await SetItemValue(itemNumber, value.ToString(CultureInfo.InvariantCulture));

        //public override async Task<bool> SetItemValue(int itemNumber, long value)
        //    => await SetItemValue(itemNumber, value.ToString());
        protected async Task<bool> WakeUpInstrument(CancellationToken ct)
        {
            await ExecuteCommandAsync(Commands.WakeupOne());
            await Task.Delay(200);

            try
            {
                var response = await ExecuteCommandAsync(Commands.WakeupTwo());

                if (response.IsSuccess || (response.ResponseCode == ResponseCode.InvalidEnquiryError))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (!(ex is TimeoutException))
                {
                    await Task.Delay(200);
                    await ExecuteCommandAsync(Commands.OkayToSend());
                }

                throw;
            }

            return false;
        }
    }

    internal class HoneywellClient : BaseHoneywellClient
    {
        internal HoneywellClient(ICommPort commPort, HoneywellDeviceType deviceType) : base(commPort, deviceType)
        {
        }
    }
}