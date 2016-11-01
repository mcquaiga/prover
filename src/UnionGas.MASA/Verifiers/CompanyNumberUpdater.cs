using System.Threading.Tasks;
using Caliburn.Micro;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.Common;
using UnionGas.MASA.Dialogs.CompanyNumberDialog;
using LogManager = NLog.LogManager;

namespace UnionGas.MASA.Verifiers
{
    public class CompanyNumberUpdater : IUpdater
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IInstrumentStore<Instrument> _instrumentStore;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ScreenManager _screenManager;

        public CompanyNumberUpdater(ScreenManager screenManager, IEventAggregator eventAggregator,
            IInstrumentStore<Instrument> instrumentStore)
        {
            _screenManager = screenManager;
            _eventAggregator = eventAggregator;
            _instrumentStore = instrumentStore;
        }

        public async Task<object> Update(EvcCommunicationClient evcCommunicationClient, Instrument instrument)
        {
            var newCompanyNumber = GetNewValue();

            await evcCommunicationClient.Connect();
            var response =
                await
                    evcCommunicationClient.SetItemValue(ItemCodes.SiteInfo.CompanyNumber, long.Parse(newCompanyNumber));
            await evcCommunicationClient.Disconnect();

            if (response)
            {
                instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber).RawValue = newCompanyNumber;
                await _instrumentStore.UpsertAsync(instrument);
            }

            return newCompanyNumber;
        }

        private string GetNewValue()
        {
            var dialog = _screenManager.ResolveViewModel<CompanyNumberDialogViewModel>();
            var result = _screenManager.ShowDialog(dialog);

            if (result.HasValue && result.Value)
            {
                _log.Debug($"New company number {dialog.CompanyNumber} was entered.");
                return dialog.CompanyNumber;
            }

            return null;
        }
    }
}