using System;
using System.Diagnostics;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.Common;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Dialogs.CompanyNumberDialog;
using LogManager = NLog.LogManager;

namespace UnionGas.MASA.Verifiers
{
    public class CompanyNumberVerifier : IVerifier
    {
        private Logger _log = LogManager.GetCurrentClassLogger();

        public CompanyNumberVerifier(IWindowManager windowManager, IInstrumentStore<Instrument> instrumentStore, DCRWebServiceSoap webService)
        {
            WindowManager = windowManager;
            InstrumentStore = instrumentStore;
            WebService = webService;
        }

        public IInstrumentStore<Instrument> InstrumentStore { get; set; }

        public IWindowManager WindowManager { get; set; }

        public DCRWebServiceSoap WebService { get; }

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
            while (true)
            {
                var viewModel = new CompanyNumberDialogViewModel();
                ScreenManager.ShowDialog(WindowManager, viewModel);

                var companyNumber = viewModel.CompanyNumber;

                _log.Debug($"New company number entered: {companyNumber}");

                long result;
                if (long.TryParse(companyNumber, out result))
                {
                    return companyNumber;
                }
            }
        }

        private async Task<MeterDTO> VerifyWithWebService(string companyNumber)
        {
            return await Task.Run(() =>
            {
                var request = new GetValidatedEvcDeviceByInventoryCodeRequest
                {
                    Body = new GetValidatedEvcDeviceByInventoryCodeRequestBody(companyNumber)
                };

                var response = WebService.GetValidatedEvcDeviceByInventoryCode(request);
                return response.Body.GetValidatedEvcDeviceByInventoryCodeResult;
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
                await InstrumentStore.UpsertAsync(instrument);
            }
            
            return response;
        }
    }
}
