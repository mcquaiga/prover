using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.Common.Messaging;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.CommProtocol.MiHoneywell.Messaging.Requests;
using Prover.CommProtocol.MiHoneywell.Messaging.Response;

namespace Prover.CommProtocol.MiHoneywell.CommClients
{
    public abstract class HoneywellClientBase : EvcCommunicationClient
    {
        internal HoneywellClientBase(CommPort commPort, InstrumentType instrumentType) : base(commPort)
        {
            InstrumentType = instrumentType;
            ItemDetails = ItemHelpers.LoadItems(InstrumentType);
        }
        
        public InstrumentType InstrumentType { get; }

        public override IEnumerable<ItemMetadata> ItemDetails { get; }

        public override bool IsConnected { get; protected set; }

        /// <summary>
        /// Establishes a link with the instrument
        /// </summary>
        public override async Task Connect(int retryAttempts = MaxConnectionAttempts, CancellationTokenSource cancellationToken = null)
        {
            var connectionAttempts = 0;
            if (!IsConnected)
            {
                do
                {
                    connectionAttempts++;
                    var response = await WakeUpInstrument();
                    if (response.IsSuccess)
                    {
                        Thread.Sleep(300);
                        response = await ExecuteCommand(Commands.SignOnCommand(InstrumentType), ResponseProcessors.ResponseCode);

                        if (response.IsSuccess)
                        {
                            Log.Debug("Connected!");
                            IsConnected = true;
                        }
                    }

                    if (!response.IsSuccess)
                    {
                        Log.Warn($"Error connecting to instrument - {response.ResponseCode}");
                        Thread.Sleep(1000);
                    }

                } while (!IsConnected && connectionAttempts < MaxConnectionAttempts);
            }
        }

        public override async Task Disconnect()
        {
            var response = await ExecuteCommand(Commands.SignOffCommand(), ResponseProcessors.ResponseCode);

            if (response.IsSuccess)
            {
                await CommPort.CloseAsync();
                IsConnected = false;
            }
        }

        public override async Task<object> GetItemValue(int itemNumber)
        {
            await Connect();

            return await ExecuteCommand(Commands.ReadItemCommand(itemNumber), ResponseProcessors.ItemValue);
        }

        public override async Task<object> GetItemValue(IEnumerable<int> itemNumbers)
        {
            await Connect();

            throw new System.NotImplementedException();
        }

        public override async Task<bool> SetItemValue(int itemNumber, string value)
        {
            throw new System.NotImplementedException();
        }

        public override async Task<bool> SetItemValue(int itemNumber, decimal value)
        {
            throw new System.NotImplementedException();
        }

        public override async Task<bool> SetItemValue(int itemNumber, int value)
        {
            throw new System.NotImplementedException();
        }

        private async Task<StatusResponseMessage> WakeUpInstrument()
        {
            await ExecuteCommand(Commands.WakeupOne());
            Thread.Sleep(500);
            return await ExecuteCommand(Commands.WakeupTwo(), ResponseProcessors.ResponseCode);
        }
    }
}
