using System;
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
using LogManager = Caliburn.Micro.LogManager;

namespace UnionGas.MASA.Verifiers
{
    public class CompanyNumberUpdater : IUpdater
    {
        private readonly Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private readonly ScreenManager _screenManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IInstrumentStore<Instrument> _instrumentStore;

        public CompanyNumberUpdater(ScreenManager screenManager, IEventAggregator eventAggregator, IInstrumentStore<Instrument> instrumentStore)
        {
            _screenManager = screenManager;
            _eventAggregator = eventAggregator;
            _instrumentStore = instrumentStore;
        }

        public async Task<object> Update(EvcCommunicationClient evcCommunicationClient, Instrument instrument)
        {
            var newCompanyNumber = GetNewValue();

            await evcCommunicationClient.Connect();
            var response = await evcCommunicationClient.SetItemValue(ItemCodes.SiteInfo.CompanyNumber, long.Parse((string)newCompanyNumber));
            await evcCommunicationClient.Disconnect();

            if (response)
            {
                instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber).RawValue = (string)newCompanyNumber;
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