using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.CommPorts;
using Prover.CommProtocol.Common.Messaging;
using Prover.CommProtocol.MiHoneywell.Messaging.Requests;
using Prover.CommProtocol.MiHoneywell.Messaging.Response;

namespace Prover.CommProtocol.MiHoneywell.CommClients
{
    public abstract class HoneywellClientBase : EvcCommunicationClient
    {
        private const int MaxConnectionAttempts = 10;

        internal HoneywellClientBase(CommPort commPort, InstrumentType instrumentType) : base(commPort)
        {
            InstrumentType = instrumentType;
        }
        
        public InstrumentType InstrumentType { get; }

        public override bool IsConnected { get; protected set; }

        protected virtual async Task ExecuteCommand(string cmd)
        {
            await CommPort.SendAsync(cmd);
        }

        protected virtual async Task<T> ExecuteCommand<T>(string cmd, ResponseProcessor<T> processor = null)
        {
            await CommPort.SendAsync(cmd);

            if (processor != null)
            {
                var messageSourceConnectable = processor.ResponseObservable(CommPort.DataReceivedObservable).Publish();

                using (messageSourceConnectable.Connect())
                {
                    return await messageSourceConnectable
                        .Timeout(TimeSpan.FromSeconds(2))
                        .FirstAsync();
                }
            }

            return default(T);
        } 

        /// <summary>
        /// Establishes a link with the instrument
        /// </summary>
        public override async Task Connect(CancellationTokenSource cancellationToken = null)
        {
            var connectionAttempts = 0;
            if (!IsConnected)
            {
                do
                {
                    await WakeUpInstrument();
                    var response = await ExecuteCommand(Commands.SignOnCommand(InstrumentType), ResponseProcessors.ResponseCode);

                    if (response.Item1 == ResponseCode.NoError)
                        IsConnected = true;
                    else
                        connectionAttempts++;

                } while (!IsConnected && connectionAttempts < MaxConnectionAttempts);
            }
        }

        public override async Task Disconnect()
        {
            var response = await ExecuteCommand(Commands.SignOffCommand(), ResponseProcessors.ResponseCode);
            
            IsConnected = false;
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

        protected virtual async Task WakeUpInstrument()
        {
            await ExecuteCommand(Commands.WakeupOne());
            var response = await ExecuteCommand(Commands.WakeupTwo(), ResponseProcessors.Acknowledgment);
        }
    }
}
