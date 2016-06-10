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
        private const int ConnectionRetryDelayMs = 3000;

        public HoneywellClient(CommPort commPort, InstrumentType instrumentType) : base(commPort)
        {
            InstrumentType = instrumentType;
            ItemDetails = ItemHelpers.LoadItems(InstrumentType);
        }

        public InstrumentType InstrumentType { get; }

        public override IEnumerable<ItemMetadata> ItemDetails { get; }

        public override bool IsConnected { get; protected set; }
        public bool IsAwake { get; private set; } = false;

        /// <summary>
        ///     Establishes a link with the instrument
        /// </summary>
        public override async Task Connect(int retryAttempts = 10, CancellationTokenSource cancellationToken = null)
        {
            var connectionAttempts = 0;

            if (cancellationToken == null) 
                cancellationToken = new CancellationTokenSource();

            var ct = cancellationToken.Token;

            var result = Task.Run(async () =>
            {
                while (!IsConnected)
                {
                    connectionAttempts++;
                    Log.Info(
                        $"[{CommPort.Name}] Connecting to {InstrumentType}... Attempt {connectionAttempts} of {MaxConnectionAttempts}");

                    try
                    {
                        
                        if (!CommPort.IsOpen())
                            await CommPort.OpenAsync();

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
                                {
                                    await CommPort.CloseAsync();
                                }

                                throw new Exception($"Error response {response.ResponseCode}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Warn(ex.Message);
                    }

                    if (!IsConnected)
                    {
                        if (connectionAttempts < retryAttempts)
                        {
                            Thread.Sleep(ConnectionRetryDelayMs);
                        }
                        else
                        {
                            throw new Exception($"{CommPort.Name} Could not connect to {InstrumentType}!");
                        }
                    }
                }
            }, ct);

            try
            {
                await result;
            }
            catch (AggregateException e)
            {
                foreach (var v in e.InnerExceptions)
                    Log.Warn(e.Message + " " + v.Message);
            }
            finally
            {
                cancellationToken.Dispose();
            }
        }

        public override async Task Disconnect()
        {
            if (IsConnected)
            {
                var response = await ExecuteCommand(Commands.SignOffCommand());

                if (response.IsSuccess)
                {
                    //await CommPort.CloseAsync();
                    IsConnected = false;
                }
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
            
            while(true)
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

        public override async Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<ItemMetadata> itemNumbers) => await GetItemValues(itemNumbers.GetAllItemNumbers());

        public override async Task<bool> SetItemValue(int itemNumber, string value)
        {
            var result = await ExecuteCommand(Commands.WriteItem(itemNumber, value));

            return result.IsSuccess;
        }

        public override async Task<bool> SetItemValue(int itemNumber, decimal value) => await SetItemValue(itemNumber, value.ToString(CultureInfo.InvariantCulture));

        public override async Task<bool> SetItemValue(int itemNumber, long value) => await SetItemValue(itemNumber, value.ToString());

        public override async Task<bool> SetItemValue(string itemCode, long value) => await SetItemValue(ItemDetails.GetItem(itemCode), value);

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

            if (response.IsSuccess || response.ResponseCode == ResponseCode.InvalidEnquiryError)
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