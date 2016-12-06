using System;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.ExternalIntegrations;
using Prover.Core.ExternalIntegrations.Validators;
using Prover.Core.Login;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using UnionGas.MASA.DCRWebService;

namespace UnionGas.MASA.Validators.InventoryCode
{
    public class InventoryCodeValidator : IValidator
    {
        private readonly IInstrumentStore<Instrument> _instrumentStore;
        private readonly DCRWebServiceSoap _webService;
        private readonly IUpdater _updater;
        private readonly ILoginService<EmployeeDTO> _loginService;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public InventoryCodeValidator(IInstrumentStore<Instrument> instrumentStore, DCRWebServiceSoap webService, IUpdater updater, ILoginService<EmployeeDTO> loginService)
        {
            _instrumentStore = instrumentStore;
            _webService = webService;
            _updater = updater;
            _loginService = loginService;
        }
       
        public async Task<object> Validate(EvcCommunicationClient commClient, Instrument instrument)
        {
            var companyNumberItem = instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber);
            var companyNumber = companyNumberItem.RawValue.TrimStart('0');

            MeterDTO meterDto;
            do
            {
                meterDto = await VerifyWithWebService(companyNumber);

                if (meterDto != null && (meterDto?.InventoryCode == null || meterDto.InventoryCode != companyNumber))
                {
                    _log.Warn($"Inventory code {companyNumber} not found in an open job.");
                    companyNumber = (string) await _updater.Update(commClient, instrument);
                }
                else
                {
                    break;
                }
            } while (companyNumber != null);
            

            await UpdateInstrumentValues(instrument, meterDto);

            return meterDto;
        }

        private async Task UpdateInstrumentValues(Instrument instrument, MeterDTO meterDto)
        {
            instrument.JobId = meterDto?.JobNumber.ToString();
            instrument.EmployeeId = _loginService.User?.Id;
            await _instrumentStore.UpsertAsync(instrument);
        }

        public async Task<MeterDTO> VerifyWithWebService(string companyNumber)
        {
            var tokenSource = new CancellationTokenSource(1500);
            var token = tokenSource.Token;

            _log.Debug($"Verifying company number {companyNumber} with web service.");

            try
            {
                var request = new GetValidatedEvcDeviceByInventoryCodeRequest
                {
                    Body = new GetValidatedEvcDeviceByInventoryCodeRequestBody(companyNumber)
                };
                
                var response = await Task.Run(async () => await _webService.GetValidatedEvcDeviceByInventoryCodeAsync(request), token);
                return response.Body.GetValidatedEvcDeviceByInventoryCodeResult;
            }
            catch (EndpointNotFoundException)
            {
                _log.Warn($"Web service not available. Skipping company number verification.");
                return null;
            }
        }
    }
}