using Prover.CommProtocol.Common;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Services;
using Prover.Core.VerificationTests.TestActions;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.Core.Modules.Clients.VerificationTestActions
{
    public class ClientPostVolumeResetAction : IEvcDeviceValidationAction
    {
        private readonly IClientService _clientService;
        private readonly TestRunService _testRunService;

        public ClientPostVolumeResetAction(IClientService clientService, TestRunService testRunService)
        {
            _clientService = clientService;
            _testRunService = testRunService;
        }

        public VerificationStep VerificationStep => VerificationStep.PostVolumeVerification;

        public async Task Execute(EvcCommunicationClient commClient, Instrument instrument, CancellationToken ct = new CancellationToken(), Subject<string> statusUpdates = null)
        {
            if (instrument.Client == null)
            {
                return;
            }

            Client client = await _clientService.GetById(instrument.Client.Id).ConfigureAwait(false);

            System.Collections.Generic.List<CommProtocol.Common.Items.ItemValue> resetItems = client.Items
                .Find(c => c.ItemFileType == ClientItemType.Reset
                                     && c.InstrumentType == instrument.InstrumentType)
                ?.Items.ToList();

            if (resetItems?.Any() != true)
            {
                return;
            }

            await commClient.Connect(ct).ConfigureAwait(false);

            int progress = 1;
            foreach (CommProtocol.Common.Items.ItemValue item in resetItems)
            {
                statusUpdates?.OnNext($"Resetting items... {progress} of {resetItems.Count}");
                bool response = await commClient.SetItemValue(item.Metadata.Number, item.RawValue).ConfigureAwait(false);
                progress++;
            }
            await commClient.Disconnect().ConfigureAwait(false);
        }
    }
}