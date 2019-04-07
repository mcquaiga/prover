using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Services;
using Prover.Core.VerificationTests.TestActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.Core.Modules.Clients.VerificationTestActions
{
    public class ItemVerificationManager : IEvcDeviceValidationAction
    {
        private readonly IHandleInvalidItemVerification _invalidItemHandler;
        private readonly IClientService _clientService;
        private readonly TestRunService _testRunService;

        public ItemVerificationManager(IHandleInvalidItemVerification invalidItemHandler, IClientService clientService,
            TestRunService testRunService)
        {
            _invalidItemHandler = invalidItemHandler;
            _clientService = clientService;
            _testRunService = testRunService;
        }

        private async Task<bool> IsValid(Instrument instrument)
        {
            if (instrument.Client == null)
            {
                return true;
            }

            Client client = await _clientService.GetById(instrument.Client.Id);

            InvalidInstrumentValues.Clear();
            ValidationItems.Clear();

            ValidationItems = client.Items
                .Find(c => c.ItemFileType == ClientItemType.Verify && c.InstrumentType == instrument.InstrumentType)?
                .Items
                .ToList();

            if (ValidationItems == null)
            {
                return true;
            }

            foreach (ItemValue validItem in ValidationItems)
            {
                ItemValue instrumentItem =
                    instrument.Items.FirstOrDefault(i => i.Metadata.Number == validItem.Metadata.Number);

                if (instrumentItem != null && !Equals(instrumentItem.NumericValue, validItem.NumericValue))
                {
                    Tuple<ItemValue, ItemValue> values = new Tuple<ItemValue, ItemValue>(validItem, instrumentItem);
                    InvalidInstrumentValues.Add(validItem.Metadata, values);
                }
            }

            return InvalidInstrumentValues.Count == 0;

        }

        /// <summary>
        ///     Contains the ItemMetada and the valid & invalid value for that item
        ///     T1 = Client value (Expected)
        ///     T2 = Instrument value (Actual)
        /// </summary>
        public Dictionary<ItemMetadata, Tuple<ItemValue, ItemValue>> InvalidInstrumentValues =
            new Dictionary<ItemMetadata, Tuple<ItemValue, ItemValue>>();

        public List<ItemValue> ValidationItems { get; private set; } = new List<ItemValue>();

        public VerificationStep VerificationStep => VerificationStep.PreVerification;

        private async Task<object> Update(EvcCommunicationClient evcCommunicationClient, Instrument instrument)
        {
            foreach (KeyValuePair<ItemMetadata, Tuple<ItemValue, ItemValue>> invalidItem in InvalidInstrumentValues)
            {
                bool response =
                    await evcCommunicationClient.SetItemValue(invalidItem.Key.Number, invalidItem.Value.Item1.RawValue);
                if (response)
                {
                    instrument.Items.First(i => i.Metadata.Number == invalidItem.Key.Number).RawValue =
                        invalidItem.Value.Item1.RawValue;
                }
            }
            return true;
        }

        public async Task Execute(EvcCommunicationClient commClient, Instrument instrument, CancellationToken ct = default, Subject<string> statusUpdates = null)
        {
            if (!await IsValid(instrument))
            {
                bool result = _invalidItemHandler.ShouldInvalidItemsBeChanged(InvalidInstrumentValues);

                if (result)
                {
                    await Update(commClient, instrument).ConfigureAwait(false);
                }
            }
        }
    }
}