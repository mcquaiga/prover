using System;
using System.Linq;
using System.Reactive.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Shared.Interfaces;

namespace Prover.Modules.UnionGas.MasaWebService
{
    internal class DevVerificationInitializer : IVerificationCustomActions
    {
        private readonly ILoginService<EmployeeDTO> _loginService;

        public DevVerificationInitializer(ILoginService<EmployeeDTO> loginService)
        {
            _loginService = loginService;
        }
        public VerificationTestStep RunOnStep { get; } = VerificationTestStep.OnInitialize;
        public async Task<bool> Run(IDeviceSessionManager deviceManager, DeviceInstance device, EvcVerificationViewModel verification)
        {
            await Task.CompletedTask;
            verification.EmployeeId = _loginService.User?.Id;
            verification.JobId = "98765";
            return true;
        }
    }

    internal class MasaVerificationInitialization : IVerificationCustomActions
    {
        private const int CompanyNumberItemId = 201;
        private readonly ILogger<MasaVerificationInitialization> _logger;
        private readonly ILoginService<EmployeeDTO> _loginService;
        private readonly DCRWebServiceSoap _webService;

        public MasaVerificationInitialization(DCRWebServiceSoap webService, ILoginService<EmployeeDTO> loginService,
            ILogger<MasaVerificationInitialization> logger = null)
        {
            _webService = webService;
            _loginService = loginService;
            _logger = logger ?? NullLogger<MasaVerificationInitialization>.Instance;
        }

        public VerificationTestStep RunOnStep => VerificationTestStep.OnInitialize;

        public async Task<bool> Run(IDeviceSessionManager deviceManager, DeviceInstance device,
            EvcVerificationViewModel verification)
        {
            var meterDto = await ValidateDeviceWithMeterDto(deviceManager, device);

            verification.JobId = meterDto?.JobNumber.ToString();
            verification.EmployeeId = _loginService.User?.Id;

            return true;
        }

        private async Task UpdateCompanyNumber(IDeviceSessionManager deviceManager, DeviceInstance device,
            string companyNumber)
        {
            var companyNumberItem =
                device.DeviceType.GetItemMetadata<SiteInformationItems>().First(i => i.Number == CompanyNumberItemId);

            await deviceManager.WriteItemValue(companyNumberItem, companyNumber);
        }

        private bool Validate(MeterDTO meterDto, string companyNumber, string serialNumber)
        {
            if (string.IsNullOrEmpty(meterDto?.InventoryCode)
                || string.IsNullOrEmpty(meterDto?.SerialNumber)
                || meterDto?.InventoryCode != companyNumber
                || meterDto?.SerialNumber.TrimStart('0') != serialNumber)
                return false;

            return true;
        }

        private async Task<MeterDTO> ValidateDeviceWithMeterDto(IDeviceSessionManager deviceManager,
            DeviceInstance device)
        {
            try
            {
                var meterDto = await _webService.FindMeterByInventoryNumber(device.CompanyNumber());
                var isValid = Validate(meterDto, device.CompanyNumber(), device.Items.SiteInfo.SerialNumber);
                while (!isValid)
                {
                    var newCompanyNumber =
                        await MessageInteractions.GetInputString.Handle("Enter updated inventory #");

                    if (string.IsNullOrEmpty(newCompanyNumber))
                        return null;

                    meterDto = await _webService.FindMeterByInventoryNumber(newCompanyNumber);
                    isValid = Validate(meterDto, newCompanyNumber, device.Items.SiteInfo.SerialNumber);

                    if (isValid) await UpdateCompanyNumber(deviceManager, device, newCompanyNumber);
                };
                return meterDto;
            }
            catch (EndpointNotFoundException)
            {
                return null;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }
    }
}