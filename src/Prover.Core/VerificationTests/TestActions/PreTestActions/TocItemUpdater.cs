using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Models.Instruments;

namespace Prover.Core.VerificationTests.TestActions.PreTestActions
{
    public class TocItemUpdater : IPreTestAction
    {
        private readonly Dictionary<int, string> _itemResetValues;

        public TocItemUpdater(Dictionary<int, string> resetValues)
        {
            _itemResetValues = resetValues;
        }

        public async Task Execute(EvcCommunicationClient commClient, Instrument instrument, Subject<string> statusUpdates = null)
        {
            if (instrument.InstrumentType.Id != 33)
                return;

            foreach (var item in _itemResetValues)
            {
                await commClient.SetItemValue(item.Key, item.Value);
            }
        }
    }
}
