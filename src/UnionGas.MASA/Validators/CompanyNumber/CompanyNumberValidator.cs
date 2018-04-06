using System;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.ExternalIntegrations.Validators;
using Prover.Core.Login;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using UnionGas.MASA.DCRWebService;

namespace UnionGas.MASA.Validators.CompanyNumber
{
    public class CompanyNumberValidator : IValidator
    {
        private readonly DCRWebServiceSoap _webService;
        private readonly IUpdater _updater;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public CompanyNumberValidator(DCRWebServiceSoap webService, IUpdater updater)
        {
            _webService = webService;
            _updater = updater;
        }
       
        public async Task<object> Validate(EvcCommunicationClient commClient, Instrument instrument)
        {
            var companyNumberItem = instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber);
            var companyNumber = companyNumberItem.RawValue.TrimStart('0');

            var serialNumberItem = instrument.Items.GetItem(ItemCodes.SiteInfo.SerialNumber);
            var serialNumber = serialNumberItem.RawValue.TrimStart('0');

            MeterDTO meterDto;
            do
            {
                meterDto = await VerifyWithWebService(companyNumber);

                if (string.IsNullOrEmpty(meterDto?.InventoryCode) || string.IsNullOrEmpty(meterDto?.SerialNumber)
                    || meterDto.InventoryCode != companyNumber || meterDto.SerialNumber.TrimStart('0') != serialNumber)                   
                {
                    _log.Warn($"Company number {companyNumber} not found in an open job.");
                    companyNumber = (string) await _updater.Update(commClient, instrument);
                }
                else
                {
                    break;
                }
            } while (!string.IsNullOrEmpty(companyNumber));
            
            UpdateInstrumentValues(instrument, meterDto);

            return meterDto;
        }     

        private void UpdateInstrumentValues(Instrument instrument, MeterDTO meterDto)
        {
            instrument.JobId = meterDto?.JobNumber.ToString();
        }

        public async Task<MeterDTO> VerifyWithWebService(string companyNumber)
        {
            var tokenSource = new CancellationTokenSource(new TimeSpan(0, 0, 0, 3));
            tokenSource.Token.ThrowIfCancellationRequested();

            _log.Debug($"Verifying company number {companyNumber} with web service.");

            try
            {
                var request = new GetValidatedEvcDeviceByInventoryCodeRequest
                {
                    Body = new GetValidatedEvcDeviceByInventoryCodeRequestBody(companyNumber)
                };

                var response = await Task.Run(async () => await _webService.GetValidatedEvcDeviceByInventoryCodeAsync(request), tokenSource.Token);
                return response.Body.GetValidatedEvcDeviceByInventoryCodeResult;
            }
            catch (OperationCanceledException)
            {
                _log.Warn($"Timed out contacting the web service.");
                return null;
            }
            catch (EndpointNotFoundException)
            {
                _log.Warn($"Web service not available. Skipping company number verification.");
                return null;
            }
        }
    }
}