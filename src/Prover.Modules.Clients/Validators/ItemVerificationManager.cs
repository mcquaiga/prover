using Prover.Core.ExternalIntegrations.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.Core.Models.Clients;
using Prover.CommProtocol.Common.Items;
using Prover.GUI.Common;
using Prover.Modules.Clients.Screens.ItemValidation;

namespace Prover.Modules.Clients.Validators
{
    public class ItemVerificationManager : IValidator, IUpdater
    {
        private readonly IProverStore<Client> _clientStore;
        private readonly IProverStore<Instrument> _instrumentStore;
        private readonly ScreenManager _screenManager;
        private List<ItemValue> _clientValidationItems = new List<ItemValue>();

        public ItemVerificationManager(IProverStore<Client> clientStore, IProverStore<Instrument> instrumentStore, ScreenManager screenManager)
        {
            _clientStore = clientStore;
            _instrumentStore = instrumentStore;
            _screenManager = screenManager;
        }

        public async Task<object> Validate(EvcCommunicationClient evcCommunicationClient, Instrument instrument)
        {
            InvalidInstrumentValues.Clear();

            _clientValidationItems.Clear();
            _clientValidationItems = instrument.Client.Items
                .FirstOrDefault(c => c.ItemFileType == ClientItemType.Verify && c.InstrumentType == instrument.InstrumentType)?
                .Items
                .ToList();

            if (_clientValidationItems == null) return 0;

            foreach (var validItem in _clientValidationItems)
            {
                var instrumentItem = instrument.Items.FirstOrDefault(i => i.Metadata.Number == validItem.Metadata.Number);

                if (instrumentItem != null && !Equals(instrumentItem.NumericValue, validItem.NumericValue))
                {
                    var values = new Tuple<ItemValue, ItemValue>(validItem, instrumentItem);
                    InvalidInstrumentValues.Add(validItem.Metadata, values);
                }
            }

            await ShowItemDialog(evcCommunicationClient, instrument);

            return InvalidInstrumentValues.Count();            
        }

        private async Task ShowItemDialog(EvcCommunicationClient evcCommunicationClient, Instrument instrument)
        {
            if (InvalidInstrumentValues.Any())
            {
                //show dialog
                var dialog = _screenManager.ResolveViewModel<ItemValidationViewModel>();
                dialog.ItemVerificationManager = this;

                var result = _screenManager.ShowDialog(dialog);
                if (result.HasValue && result.Value)
                {
                    await Update(evcCommunicationClient, instrument);
                }
            }
        }

        /// <summary>
        /// Contains the ItemMetada and the valid & invalid value for that item
        /// T1 = Client value
        /// T2 = Instrument value
        /// </summary>
        public Dictionary<ItemMetadata, Tuple<ItemValue, ItemValue>> InvalidInstrumentValues = new Dictionary<ItemMetadata, Tuple<ItemValue, ItemValue>>();
        public List<ItemValue> ValidationItems => _clientValidationItems;

        public async Task<object> Update(EvcCommunicationClient evcCommunicationClient, Instrument instrument)
        {
            await evcCommunicationClient.Connect();
            foreach (var invalidItem in InvalidInstrumentValues)
            {
                await evcCommunicationClient.SetItemValue(invalidItem.Key.Number, invalidItem.Value.Item1.RawValue);
                instrument.Items.First(i => i.Metadata.Number == invalidItem.Key.Number).RawValue = invalidItem.Value.Item1.RawValue;
            }
            await evcCommunicationClient.Disconnect();
            await _instrumentStore.UpsertAsync(instrument);

            return true;
        }
    }
}
