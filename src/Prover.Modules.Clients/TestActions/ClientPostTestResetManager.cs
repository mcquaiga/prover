using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.Core.VerificationTests.TestActions;
using Prover.GUI.Common;

namespace Prover.Modules.Clients.TestActions
{
    public class ClientPostTestResetManager : PostTestResetBase
    {
        private IProverStore<Client> _clientStore;
        private IProverStore<Instrument> _instrumentStore;
        private ScreenManager _screenManager;

        public ClientPostTestResetManager(IProverStore<Client> clientStore, IProverStore<Instrument> instrumentStore, ScreenManager screenManager)
        {
            _clientStore = clientStore;
            _instrumentStore = instrumentStore;
            _screenManager = screenManager;
        }

        public override async Task Execute(EvcCommunicationClient commClient, Instrument instrument)
        {
            var resetItems = instrument.Client.Items.FirstOrDefault(c => c.ItemFileType == ClientItemType.Reset && c.InstrumentType == instrument.InstrumentType)?.Items.ToList();

            if (resetItems == null || !resetItems.Any()) return;

            var cts = new CancellationTokenSource();
            await commClient.Connect(cts.Token);
            foreach (var item in resetItems)
            {
                var response = await commClient.SetItemValue(item.Metadata.Number, item.RawValue);
            }
            await commClient.Disconnect();
        }
    }
}
