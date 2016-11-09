using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.ExternalIntegrations;
using Prover.Core.ExternalIntegrations.Validators;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.Common;

namespace UnionGas.MASA.Validators.InventoryCode
{
    public class InventoryCodeUpdater : IUpdater
    {
        private readonly IInstrumentStore<Instrument> _instrumentStore;
        private readonly IGetValue _valueRequestor;
        private readonly LoginService _loginService;

        public InventoryCodeUpdater(IInstrumentStore<Instrument> instrumentStore, IGetValue valueRequestor, LoginService loginService)
        {
            _instrumentStore = instrumentStore;
            _valueRequestor = valueRequestor;
            _loginService = loginService;
        }

        public async Task<object> Update(EvcCommunicationClient evcCommunicationClient, Instrument instrument)
        {
            var newCompanyNumber = _valueRequestor.GetValue();

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
            else
            {
                //TODO throw exception that the inventory couldn't be updated
            }

            return newCompanyNumber;
        }
    }
}