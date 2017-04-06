using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.Instruments;
using Prover.CommProtocol.MiHoneywell.Messaging.Requests;

namespace Prover.CommProtocol.MiHoneywell.CommClients
{
    internal class HoneywellClient : EvcCommunicationClient
    {
        public HoneywellClient(CommPort commPort) : base(commPort)
        {
        }

        public override bool IsConnected { get; protected set; }
        public bool IsAwake { get; private set; }

        /// <summary>
        ///     Establishes a link with the instrument
        /// </summary>
        protected override async Task ConnectToInstrument<T>(T instrument)
        {            
            await WakeUpInstrument();

            if (IsAwake)
            {
                var response = await ExecuteCommand(Commands.SignOn(instrument.Id));

                if (response.IsSuccess)
                {
                    IsConnected = true;
                    Log.Info($"[{CommPort.Name}] Connected to {instrument.Name}!");
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

        public override async Task<ItemValue> GetItemValue(ItemMetadata item)
        {
            var response = await ExecuteCommand(Commands.ReadItem(item.Number));
            return new ItemValue(item, response.RawValue);
        }

        public override async Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<ItemMetadata> items)
        {
            var results = new List<ItemValue>();
            
            items = items.OrderBy(x => x.Number).ToList();

            var y = 0;

            while (true)
            {
                var set = items
                            .Skip(y)
                            .Take(15)
                            .Select(i => i.Number)
                            .ToList();

                if (!set.Any())
                    break;

                var response = await ExecuteCommand(Commands.ReadGroup(set));
                foreach (var item in response.ItemValues)
                {
                    var metadata = items.FirstOrDefault(x => x.Number == item.Key);
                    results.Add(new ItemValue(metadata, item.Value));
                }

                y += 15;
            }

            return results;
        }

        public override async Task<bool> SetItemValue(ItemMetadata item, string value)
        {
            var result = await ExecuteCommand(Commands.WriteItem(item.Number, value));
            return result.IsSuccess;
        }

        public override async Task<bool> SetItemValue(ItemMetadata item, decimal value)
            => await SetItemValue(item, value.ToString(CultureInfo.InvariantCulture));

        public override async Task<bool> SetItemValue(ItemMetadata item, long value)
            => await SetItemValue(item, value.ToString());

        public override async Task<ItemValue> LiveReadItemValue(ItemMetadata item)
        {
            var response = await ExecuteCommand(Commands.LiveReadItem(item.Number));
            return new ItemValue(item, response.RawValue);
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