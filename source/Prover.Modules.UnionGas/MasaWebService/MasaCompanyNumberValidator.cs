using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Modules.UnionGas.DcrWebService;

namespace Prover.Modules.UnionGas.MasaWebService
{
    internal class MasaCompanyNumberValidator : IDeviceValidation
    {
        private const int CompanyNumberItemId = 201;
        private readonly DCRWebServiceSoap _webService;
        private readonly ILogger<MasaCompanyNumberValidator> _logger;

        public MasaCompanyNumberValidator(DCRWebServiceSoap webService, ILogger<MasaCompanyNumberValidator> logger = null)
        {
            _webService = webService;
            _logger = logger ?? NullLogger<MasaCompanyNumberValidator>.Instance;
        }

        public async Task<bool> Validate(IDeviceSessionManager deviceManager, DeviceInstance device)
        {
            var isValid = await ValidateWithWebService(device.CompanyNumber(), device.Items.SiteInfo.SerialNumber);
            while (!isValid)
            {
                var newCompanyNumber = 
                    await MessageInteractions.GetInputString.Handle("Enter updated inventory #");
                
                if (string.IsNullOrEmpty(newCompanyNumber)) 
                    return false;

                isValid = 
                    await ValidateWithWebService(newCompanyNumber, device.Items.SiteInfo.SerialNumber);

                if (isValid) await UpdateCompanyNumber(deviceManager, device, newCompanyNumber);
            };

            return true;
        }

        private async Task UpdateCompanyNumber(IDeviceSessionManager deviceManager, DeviceInstance device, string companyNumber)
        {
            var companyNumberItem =
                device.DeviceType.GetItemMetadata<SiteInformationItems>().First(i => i.Number == CompanyNumberItemId);

            await deviceManager.WriteItemValue(companyNumberItem, companyNumber);
        }

        private async Task<bool> ValidateWithWebService(string companyNumber, string serialNumber)
        {
            try
            {
                var meterDto = await _webService.FindMeterByCompanyNumber(companyNumber);

                if (string.IsNullOrEmpty(meterDto?.InventoryCode)
                    || string.IsNullOrEmpty(meterDto?.SerialNumber)
                    || meterDto?.InventoryCode != companyNumber
                    || meterDto?.SerialNumber.TrimStart('0') != serialNumber)
                {
                    _logger.LogWarning($"Device with inventory #{companyNumber} is not on an open job.");
                    return false;
                }
            }
            catch (EndpointNotFoundException)
            {
                return true;
            }
            catch (OperationCanceledException)
            {
                return true;
            }
            return true;
        }
    }
}
