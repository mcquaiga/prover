using System;
using System.Diagnostics;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.Common;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Dialogs.CompanyNumberDialog;

namespace UnionGas.MASA.Verifiers
{
    public class CompanyNumberVerifier : IVerifier
    {
        public CompanyNumberVerifier(IUnityContainer container, string serviceUriString)
        {
            Container = container;
            WebServiceUrl = serviceUriString;
        }

        public IUnityContainer Container { get; }
        public string WebServiceUrl { get; }

        public async Task<object> Verify(EvcCommunicationClient evcCommunicationClient, Instrument instrument)
        {
            var companyNumberItem = instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber);
            var companyNumber = companyNumberItem.ToString();

            var meterDto = await VerifyWithWebService(companyNumber);

            while (meterDto == null)
            {
                var newCompanyNumber = GetNewCompanyNumber();
                meterDto = await VerifyWithWebService(newCompanyNumber);
                if (meterDto != null)
                {
                    await UpdateInstrumentCompanyNumber(evcCommunicationClient, instrument, newCompanyNumber);
                }
            }

            return true;
        }

        private string GetNewCompanyNumber()
        {
            var viewModel = new CompanyNumberDialogViewModel(this);
            ScreenManager.ShowDialog(Container, viewModel);

            return viewModel.CompanyNumber;
        }

        private async Task<MeterDTO> VerifyWithWebService(string companyNumber)
        {
            return await Task.Run(() =>
            {
                using (var service = new DCRWebServiceSoapClient())
                {
                    service.Endpoint.Address = new EndpointAddress(WebServiceUrl);

                    return service.GetValidatedEvcDeviceByInventoryCode(companyNumber);
                }
            });
        }

        private async Task<bool> UpdateInstrumentCompanyNumber(EvcCommunicationClient commClient, Instrument instrument, string newCompanyNumber)
        {
            await commClient.Connect();
            var response = await commClient.SetItemValue(ItemCodes.SiteInfo.CompanyNumber, long.Parse(newCompanyNumber));
            await commClient.Disconnect();

            if (response)
            {
                instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber).RawValue = newCompanyNumber.ToString();
                await Container.Resolve<IInstrumentStore<Instrument>>().UpsertAsync(instrument);
            }
            
            return response;
        }
    }
}
