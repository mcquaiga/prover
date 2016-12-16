using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.ExternalIntegrations.Validators;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;

namespace UnionGas.MASA.Validators.CompanyNumber
{
    public class CompanyNumberUpdater : IUpdater
    {
        private readonly IProverStore<Instrument> _instrumentStore;
        private readonly IGetValue _valueRequestor;

        public CompanyNumberUpdater(IProverStore<Instrument> instrumentStore, IGetValue valueRequestor)
        {
            _instrumentStore = instrumentStore;
            _valueRequestor = valueRequestor;
        }

        public async Task<object> Update(EvcCommunicationClient evcCommunicationClient, Instrument instrument)
        {
            var newCompanyNumber = _valueRequestor.GetValue();
            if (string.IsNullOrEmpty(newCompanyNumber)) return string.Empty;

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