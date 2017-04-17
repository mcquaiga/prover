using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.Core.VerificationTests.TestActions;
using Prover.GUI.Common;
using Prover.Modules.Clients.Screens.ItemValidation;

namespace Prover.Modules.Clients.TestActions
{
    public class ItemVerificationManager : PreTestValidationBase
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

        public override async Task Execute(EvcCommunicationClient commClient, Instrument instrument, Subject<string> statusUpdates = null)
        {
            if (!await Validate(commClient, instrument))
            {
                var result = await ShowItemDialog();

                var ctSource = new CancellationTokenSource();
                var ct = ctSource.Token;
                if (result) await Update(commClient, instrument, ct);
            }
        }

        public async Task<bool> Validate(EvcCommunicationClient evcCommunicationClient, Instrument instrument)
        {
            return await Task.Run(() =>
            {
                if (instrument.Client == null) return true;

                InvalidInstrumentValues.Clear();

                _clientValidationItems.Clear();
                _clientValidationItems = instrument.Client.Items
                    .FirstOrDefault(c => c.ItemFileType == ClientItemType.Verify && c.InstrumentType == instrument.InstrumentType)?
                    .Items
                    .ToList();

                if (_clientValidationItems == null) return true;

                foreach (var validItem in _clientValidationItems)
                {
                    var instrumentItem = instrument.Items.FirstOrDefault(i => i.Metadata.Number == validItem.Metadata.Number);

                    if (instrumentItem != null && !Equals(instrumentItem.NumericValue, validItem.NumericValue))
                    {
                        var values = new Tuple<ItemValue, ItemValue>(validItem, instrumentItem);
                        InvalidInstrumentValues.Add(validItem.Metadata, values);
                    }
                }

                return !InvalidInstrumentValues.Any();
            });
        }

        private async Task<bool> ShowItemDialog()
        {
            if (InvalidInstrumentValues.Any())
            {
                //show dialog
                var dialog = _screenManager.ResolveViewModel<ItemValidationViewModel>();
                dialog.ItemVerificationManager = this;

                var result = _screenManager.ShowDialog(dialog);
                return result.HasValue && result.Value;
            }

            return true;
        }

        /// <summary>
        /// Contains the ItemMetada and the valid & invalid value for that item
        /// T1 = Client value
        /// T2 = Instrument value
        /// </summary>
        public Dictionary<ItemMetadata, Tuple<ItemValue, ItemValue>> InvalidInstrumentValues = new Dictionary<ItemMetadata, Tuple<ItemValue, ItemValue>>();
        public List<ItemValue> ValidationItems => _clientValidationItems;

        public async Task<object> Update(EvcCommunicationClient evcCommunicationClient, Instrument instrument, CancellationToken ct)
        {
            await evcCommunicationClient.Connect(ct);
            foreach (var invalidItem in InvalidInstrumentValues)
            {
                var response = await evcCommunicationClient.SetItemValue(invalidItem.Key.Number, invalidItem.Value.Item1.RawValue);
                if (response)
                    instrument.Items.First(i => i.Metadata.Number == invalidItem.Key.Number).RawValue = invalidItem.Value.Item1.RawValue;
            }
            await evcCommunicationClient.Disconnect();
            await _instrumentStore.UpsertAsync(instrument);

            return true;
        }

        
    }
}
