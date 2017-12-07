using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Instruments;

namespace Prover.Core.VerificationTests.TestActions.PreTestActions
{
    public class TocItemUpdater : IPreTestAction
    {
        private IEnumerable<ItemValue> _itemResetValues = new List<ItemValue>();

        public TocItemUpdater()
        {
            _itemResetValues = new List<ItemValue>()
            {
                new ItemValue(new ItemMetadata() { Number = 865 }, "")
            };
        }

        public async Task Execute(EvcCommunicationClient commClient, Instrument instrument, Subject<string> statusUpdates = null)
        {
            await commClient.SetItemValue()
        }
    }
}
