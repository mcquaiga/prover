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

namespace Prover.CommProtocol.MiHoneywell.CommClients
{
    public class HoneywellClient : EvcCommunicationClient
    {
        protected Task LoadItemsTask { get; private set; }

        public HoneywellClient(CommPort commPort, InstrumentType instrumentType) : base(commPort, instrumentType)
        {
            LoadItemsTask = Task.Run(() => ItemHelpers.LoadItems(InstrumentType))
                .ContinueWith(_ => { ItemDetails = _.Result.ToList(); });
        }

        //public override IEnumerable<ItemMetadata> ItemDetails { get; protected set; }

        public override bool IsConnected { get; protected set; }
        public bool IsAwake { get; private set; }

        /// <summary>
        ///     Establishes a link with the instrument
        /// </summary>
        protected override async Task ConnectToInstrument()
        {
            LoadItemsTask.Wait();

            await WakeUpInstrument();

            if (IsAwake)
            {
                var response = await ExecuteCommand(Commands.SignOn(InstrumentType));

                if (response.IsSuccess)
                {
                    IsConnected = true;
                    Log.Info($"[{CommPort.Name}] Connected to {InstrumentType.Name}!");
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

        public override Task<IFrequencyTestItems> GetFrequencyItems()
        {
            throw new NotImplementedException();
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

    public sealed class TocHoneywellClient : HoneywellClient
    {
        public static InstrumentType TurboMonitor = new InstrumentType()
        {
            Id = 6,
            AccessCode = 6,
            Name = "Turbo Monitor",
            ItemFilePath = "TurboMonitorItems.xml"
        };

        private readonly IEnumerable<ItemMetadata> _tibBoardItems;

        public TocHoneywellClient(CommPort commPort, InstrumentType instrumentType) : base(commPort, instrumentType)
        {
            _tibBoardItems = ItemHelpers.LoadItems(TurboMonitor);
        }

        public override async Task<IFrequencyTestItems> GetFrequencyItems()
        {
            var results = await GetItemValues(ItemDetails.FrequencyTestItems());
            await Disconnect();
            Thread.Sleep(1000);

            InstrumentType = TurboMonitor;
            await Connect();
            var tibResults = await GetItemValues(_tibBoardItems.FrequencyTestItems());
            await Disconnect();
            Thread.Sleep(1000);

            InstrumentType = Instruments.Toc;

            var values = results.ToList();
            values.AddRange(tibResults.ToList());

            return new FrequencyTestItems(values);
        }
    }
}