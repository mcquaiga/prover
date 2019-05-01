using Devices.Communications;
using Devices.Communications.IO;
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
        #region Constructors

        public HoneywellClient(ICommPort commPort, IHoneywellDeviceType instrumentType) : base(commPort, instrumentType)
        {
        }

        #endregion

        #region Properties

        public override IHoneywellDeviceType EvcDeviceType { get; set; }

        public override bool IsConnected { get; protected set; }

        #endregion

        #region Methods

        public override async Task Disconnect()
        {
            if (IsConnected)
            {
                var response = await ExecuteCommand(Commands.SignOffCommand());

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
            var response = await ExecuteCommand(Commands.ReadItem(itemNumber.Number));
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

                var response = await ExecuteCommand(Commands.ReadGroup(set));
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
            var response = await ExecuteCommand(Commands.LiveReadItem(itemNumber));
            return new ItemValue(itemDetails, response.RawValue);
        }

        public override async Task<bool> SetItemValue(int itemNumber, string value)
        {
            var result = await ExecuteCommand(Commands.WriteItem(itemNumber, value));

            return result.IsSuccess;
        }

        public override async Task<bool> SetItemValue(int itemNumber, decimal value)
            => await SetItemValue(itemNumber, value.ToString(CultureInfo.InvariantCulture));

        public override async Task<bool> SetItemValue(int itemNumber, long value)
            => await SetItemValue(itemNumber, value.ToString());

        public override async Task<bool> SetItemValue(string itemCode, long value)
            => await SetItemValue(ItemDetails.GetItem(itemCode), value);

        #endregion

        protected Task LoadItemsTask { get; private set; }

        protected override async Task ConnectToInstrument(CancellationToken ct)
        {
            if (await WakeUpInstrument(ct))
            {
                var response = await ExecuteCommand(Commands.SignOn(EvcDeviceType));

                if (response.IsSuccess)
                {
                    IsConnected = true;
                    StatusStream.OnNext($"[{CommPort.Name}] Connected to {EvcDeviceType.Name}!");
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
            await ExecuteCommand(Commands.WakeupOne());
            await Task.Delay(150);

            try
            {
                var response = await ExecuteCommand(Commands.WakeupTwo());

                if (response.IsSuccess || (response.ResponseCode == ResponseCode.InvalidEnquiryError))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                await Task.Delay(200);
                await ExecuteCommand(Commands.OkayToSend());
                throw;
            }

            return false;
        }
    }
}