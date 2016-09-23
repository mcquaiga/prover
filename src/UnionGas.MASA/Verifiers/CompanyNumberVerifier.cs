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
using Prover.GUI.Common;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Dialogs.CompanyNumberDialog;

namespace UnionGas.MASA.Verifiers
{
    public class CompanyNumberVerifier : IVerifier
    {
        public CompanyNumberVerifier(
            IUnityContainer container, 
            EvcCommunicationClient commClient,
            string serviceUriString,
            Instrument instrument)
        {
            Container = container;
            CommClient = commClient;
            WebServiceUrl = serviceUriString;
            Instrument = instrument;
        }

        public IUnityContainer Container { get; }
        public Instrument Instrument { get; }
        public EvcCommunicationClient CommClient { get; }
        public string WebServiceUrl { get; }

        public async Task<object> Verify()
        {
            var companyNumberItem = Instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber);
            var companyNumber = companyNumberItem.ToString();

            var meterDto = await VerifyWithWebService(companyNumber);

            while (meterDto == null)
            {
                var newCompanyNumber = GetNewCompanyNumber();
                meterDto = await VerifyWithWebService(newCompanyNumber);
                if (meterDto != null)
                {
                    var success = await UpdateInstrumentCompanyNumber(newCompanyNumber);
                }
            }

            return meterDto != null;
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

                    return service.GetValidatedEvcDeviceByInventoryCodeAsync(companyNumber).Result.Body.GetValidatedEvcDeviceByInventoryCodeResult;
                }
            });
        }

        public async Task<bool> UpdateInstrumentCompanyNumber(string newCompanyNumber)
        {
            await CommClient.Connect();
            var response = await CommClient.SetItemValue(ItemCodes.SiteInfo.CompanyNumber, long.Parse(newCompanyNumber));
            await CommClient.Disconnect();

            if (response)
                Instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber).RawValue = newCompanyNumber.ToString();

            return response;
        }
    }
}
