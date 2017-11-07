using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;

namespace Prover.CommProtocol.Dresser
{
    public class DresserClient : EvcCommunicationClient
    {
        public DresserClient(ICommPort commPort, InstrumentType instrumentType) : base(commPort, instrumentType)
        {
        }

        public override bool IsConnected { get; protected set; }

        protected override Task ConnectToInstrument(CancellationToken ct, string accessCode = null)
        {
            throw new NotImplementedException();
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
