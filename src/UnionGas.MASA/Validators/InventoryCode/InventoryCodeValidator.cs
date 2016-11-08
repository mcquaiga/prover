using System.ServiceModel;
using System.Threading.Tasks;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.ExternalIntegrations;
using Prover.Core.ExternalIntegrations.Validators;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using UnionGas.MASA.DCRWebService;

namespace UnionGas.MASA.Validators.InventoryCode
{
    public class InventoryCodeValidator : IValidator
    {
        private readonly IInstrumentStore<Instrument> _instrumentStore;
        private readonly IUpdater _updater;
        private readonly LoginService _loginService;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public InventoryCodeValidator(IInstrumentStore<Instrument> instrumentStore, DCRWebServiceSoap webService, IUpdater updater, LoginService loginService)
        {
            _instrumentStore = instrumentStore;
            _updater = updater;
            WebService = webService;
        }

        public InventoryCodeValidator(DCRWebServiceSoap webService)
        {
            WebService = webService;
        }

        public IUpdater Updater { get; set; }

        public IInstrumentStore<Instrument> InstrumentStore { get; set; }

        public DCRWebServiceSoap WebService { get; }

        public async Task<object> Validate(EvcCommunicationClient commClient, Instrument instrument)
        {
            var companyNumberItem = instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber);
            var companyNumber = companyNumberItem.RawValue;

            MeterDTO meterDto;
            do
            {
                meterDto = await VerifyWithWebService(companyNumber);

                if (meterDto?.InventoryCode == null || meterDto.InventoryCode != companyNumber)
                {
                    _log.Warn($"Inventory code {companyNumber} not found in an open job.");
                    companyNumber = (string) await Updater.Update(commClient, instrument);
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
            instrument.EmployeeId = _loginService.User.Id;
            await _instrumentStore.UpsertAsync(instrument);
        }

        public async Task<MeterDTO> VerifyWithWebService(string companyNumber)
        {
            _log.Debug($"Verifying company number {companyNumber} with web service.");

            try
            {
                var request = new GetValidatedEvcDeviceByInventoryCodeRequest
                {
                    Body = new GetValidatedEvcDeviceByInventoryCodeRequestBody(companyNumber)
                };

                var response = await WebService.GetValidatedEvcDeviceByInventoryCodeAsync(request);
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