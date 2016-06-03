using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.CommProtocol.MiHoneywell.Messaging.Requests;
using Prover.CommProtocol.MiHoneywell.Messaging.Response;

namespace Prover.CommProtocol.MiHoneywell.CommClients
{
    public abstract class MiClientBase : EvcCommunicationClient
    {
        private const int ConnectionRetryDelayMs = 3000;

        internal MiClientBase(CommPort commPort, InstrumentType instrumentType) : base(commPort)
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
        public override async Task Connect(int retryAttempts, CancellationTokenSource cancellationToken = null)
        {
            var connectionAttempts = 0;

            while (!IsConnected)
            {
                connectionAttempts++;
                Log.Info($"Connecting to {InstrumentType}... Attempt {connectionAttempts} of {MaxConnectionAttempts}");

                try
                {
                    await Connect();
                }
                catch (Exception ex)
                {
                    Log.Warn(ex.Message);
                }

                if (!IsConnected)
                {
                    if (connectionAttempts < MaxConnectionAttempts)
                    {
                        Thread.Sleep(ConnectionRetryDelayMs);
                    }
                    else
                    {
                        throw new Exception("Could not connect to instrument!");
                    }
                }
            }
        }

        public override async Task<bool> Connect(CancellationTokenSource cancellationToken = null)
        {
            if (IsConnected) return true;

            if (!CommPort.IsOpen())
                await CommPort.OpenAsync();

            await WakeUpInstrument();
            
            if (IsAwake)
            {
                var response = await ExecuteCommand(Commands.SignOn(InstrumentType));

                if (response.IsSuccess)
                {
                    IsConnected = true;
                    Log.Debug($"Connected to {InstrumentType} on {CommPort.Name}!");
                }
                else
                {
                    throw new Exception($"Error response {response.ResponseCode}");
                }
            }

            return IsConnected;
        }

        public override async Task Disconnect()
        {
            if (IsConnected)
            {
                var response = await ExecuteCommand(Commands.SignOffCommand());

                if (response.IsSuccess)
                {
                    await CommPort.CloseAsync();
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
            throw new NotImplementedException();
            var results = new List<ItemValue>();
            var items = itemNumbers.ToArray();
            items = items.OrderBy(x => x).ToArray();

            var y = 0;
            var set = items.Skip(y).Take(15).ToList();
            while (set.Any())
            {
                var response = await ExecuteCommand(Commands.ReadGroup(set));
                //response.ItemValues
            }
        }

        public override async Task<bool> SetItemValue(int itemNumber, string value)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> SetItemValue(int itemNumber, decimal value)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> SetItemValue(int itemNumber, int value)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> WakeUpInstrument()
        {
            await ExecuteCommand(Commands.WakeupOne());
            Thread.Sleep(150);
            var response = await ExecuteCommand(Commands.WakeupTwo());

            if (response.IsSuccess)
            {
                IsAwake = true;
                Thread.Sleep(100);
            }

            return IsAwake;
        }
    }
}