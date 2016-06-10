using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.Simulator.IO;

namespace Prover.CommProtocol.MiHoneywell.Simulator
{
    public class SimulatorClient : EvcCommunicationClient
    {
        public SimulatorClient() : base(new SimulatorPort("Simulator"))
        {
        }

        public override bool IsConnected { get; protected set; }

        public override async Task Connect(int retryAttempts = 10, CancellationTokenSource cancellationToken = null)
        {
            await Task.Run(() => Log.Debug("Connecting on simulator."));
        }

        public override Task Disconnect()
        {
            throw new NotImplementedException();
        }

        public override Task<ItemValue> GetItemValue(int itemNumber)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<int> itemNumbers)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<ItemMetadata> itemNumbers)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> SetItemValue(int itemNumber, string value)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> SetItemValue(int itemNumber, decimal value)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> SetItemValue(int itemNumber, long value)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> SetItemValue(string itemCode, long value)
        {
            throw new NotImplementedException();
        }

        public override Task<ItemValue> LiveReadItemValue(int itemNumber)
        {
            throw new NotImplementedException();
        }
    }
}
