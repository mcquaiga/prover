using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Dialogs.CompanyNumberDialog;
using LogManager = NLog.LogManager;

namespace UnionGas.MASA.Verifiers
{
    public class CompanyNumberVerifierUpdater : VerifierUpdaterBase
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public CompanyNumberVerifierUpdater(IInstrumentStore<Instrument> instrumentStore, DCRWebServiceSoap webService, IUpdater updater)
        {
            InstrumentStore = instrumentStore;
            WebService = webService;
            Updater = updater;
        }

        public IUpdater Updater { get; set; }

        public IInstrumentStore<Instrument> InstrumentStore { get; set; }

        public DCRWebServiceSoap WebService { get; }

        protected override async Task<object> Verify(EvcCommunicationClient commClient, Instrument instrument)
        {
            var companyNumberItem = instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber);
            var companyNumber = companyNumberItem.ToString();

            var meterDto = await VerifyWithWebService(companyNumber);

            while (meterDto == null)
            {
                var newCompanyNumber = await Updater.GetNewValue();

                meterDto = await VerifyWithWebService(newCompanyNumber.ToString());

                if (meterDto != null)
                    await UpdateInstrumentCompanyNumber(commClient, instrument, newCompanyNumber.ToString());
            }

            return meterDto;
        }

        protected override Task<object> Update(EvcCommunicationClient commClient, Instrument instrument)
        {
            return null;
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

        private async Task<bool> UpdateInstrumentCompanyNumber(EvcCommunicationClient commClient, Instrument instrument,
            string newCompanyNumber)
        {
            await commClient.Connect();
            var response = await commClient.SetItemValue(ItemCodes.SiteInfo.CompanyNumber, long.Parse(newCompanyNumber));
            await commClient.Disconnect();

            if (response)
            {
                instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber).RawValue = newCompanyNumber;
                await InstrumentStore.UpsertAsync(instrument);
            }

            return response;
        }
    }
}