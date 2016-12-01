using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.CommProtocol.MiHoneywell.Messaging.Requests;

namespace Prover.CommProtocol.MiHoneywell
{
    public class HoneywellClient : EvcCommunicationClient
    {
        public HoneywellClient(CommPort commPort) : base(commPort)
        {
        }

        public override IEnumerable<ItemMetadata> ItemDetails { get; protected set; }

        public override bool IsConnected { get; protected set; }
        public bool IsAwake { get; private set; }

        /// <summary>
        ///     Establishes a link with the instrument
        /// </summary>
        protected override async Task ConnectToInstrument()
        {
            if (ItemDetails == null)
            {
                var itemTask =
                    Task.Run(() => ItemHelpers.LoadItems(InstrumentType)).ContinueWith(_ => { ItemDetails = _.Result; });
            }

            await WakeUpInstrument();

            if (IsAwake)
            {
                var response = await ExecuteCommand(Commands.SignOn(InstrumentType));

                if (response.IsSuccess)
                {
                    IsConnected = true;
                    Log.Info($"[{CommPort.Name}] Connected to {InstrumentType}!");
                }
                else
                {
                    if (response.ResponseCode == ResponseCode.FramingError)
                        await CommPort.Close();

                    throw new Exception($"Error response {response.ResponseCode}");
                }
            }
        }

        public override async Task Disconnect()
        {
            if (IsConnected)
            {
                var response = await ExecuteCommand(Commands.SignOffCommand());

                if (response.IsSuccess)
                    IsConnected = false;
            }
        }

        public override async Task<ItemValue> GetItemValue(int itemNumber)
        {
            var itemDetails = ItemDetails.GetItem(itemNumber);
            var response = await ExecuteCommand(Commands.ReadItem(itemNumber));
            return new ItemValue(itemDetails, response.RawValue);
        }

        public override async Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<int> itemNumbers)
        {
            var results = new List<ItemValue>();
            var items = itemNumbers.ToArray();
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
                    var metadata = ItemDetails.FirstOrDefault(x => x.Number == item.Key);
                    results.Add(new ItemValue(metadata, item.Value));
                }

                y += 15;
            }

            return results;
        }

        public override async Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<ItemMetadata> itemNumbers)
            => await GetItemValues(itemNumbers.GetAllItemNumbers());

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

        public override async Task<ItemValue> LiveReadItemValue(int itemNumber)
        {
            var itemDetails = ItemDetails.GetItem(itemNumber);
            var response = await ExecuteCommand(Commands.LiveReadItem(itemNumber));
            return new ItemValue(itemDetails, response.RawValue);
        }

        private async Task<bool> WakeUpInstrument()
        {
            await ExecuteCommand(Commands.WakeupOne());
            Thread.Sleep(150);
            var response = await ExecuteCommand(Commands.WakeupTwo());

            if (response.IsSuccess || (response.ResponseCode == ResponseCode.InvalidEnquiryError))
            {
                IsAwake = true;
                Thread.Sleep(100);
            }
            else
            {
                await ExecuteCommand(Commands.OkayToSend());
            }

            return IsAwake;
        }
    }
}