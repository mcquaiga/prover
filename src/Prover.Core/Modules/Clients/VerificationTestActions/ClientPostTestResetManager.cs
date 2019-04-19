using System;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Services;
using Prover.Core.VerificationTests.TestActions;

namespace Prover.Core.Modules.Clients.VerificationTestActions
{
    public class ClientPostTestResetManager : IPostTestAction
    {
        private readonly IClientService _clientService;
        private readonly TestRunService _testRunService;

        public ClientPostTestResetManager(IClientService clientService, TestRunService testRunService)
        {
            _clientService = clientService;
            _testRunService = testRunService;
        }

        public Task Execute(Action<EvcCommunicationClient, Instrument> postTestAction)
        {
            throw new NotImplementedException();
        }

        public async Task Execute(EvcCommunicationClient commClient, Instrument instrument,
            Subject<string> statusUpdates)
        {
            if (instrument.Client == null)
                return;

            var client = await _clientService.GetById(instrument.Client.Id);

            var resetItems = client.Items
                .FirstOrDefault(c => c.ItemFileType == ClientItemType.Reset &&
                                     c.InstrumentType == instrument.InstrumentType)
                ?.Items.ToList();

            if (resetItems == null || resetItems.Count == 0) 
                return;

            var cts = new CancellationTokenSource();
            await commClient.Connect(cts.Token);

            var progress = 1;
            foreach (var item in resetItems)
            {
                statusUpdates?.OnNext($"Resetting items... {progress} of {resetItems.Count}");
                var response = await commClient.SetItemValue(item.Metadata.Number, item.RawValue);
                progress++;
            }
            await commClient.Disconnect();
        }
    }
}