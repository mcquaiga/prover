using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using Prover.Modules.UnionGas.DcrWebService;

namespace Prover.Modules.UnionGas.MasaWebService
{
    internal class MeterInventoryNumberValidator
    {
        private const int CompanyNumberItemId = 201;
        private readonly IDeviceSessionManager _deviceManager;
        private readonly IMeterService<MeterDTO> _meterService;

        public MeterInventoryNumberValidator(IDeviceSessionManager deviceManager, IMeterService<MeterDTO> meterService)
        {
            _deviceManager = deviceManager;
            _meterService = meterService;
        }

        public async Task<MeterDTO> Validate(EvcVerificationViewModel verification)
        {
            return await CheckInventoryNumberByMeter(verification.Device);
        }

        private async Task<MeterDTO> CheckInventoryNumberByMeter(DeviceInstance device)
        {
            var meterDto = await FindAndValidateMeterDto(device, device.CompanyNumber());

            while (meterDto == null)
            {
                var newCompanyNumber =
                    await MessageInteractions.GetInputString.Handle(
                        $"Device inventory number value was not found on an open job. {Environment.NewLine}" +
                        "Enter updated inventory number:");

                if (string.IsNullOrEmpty(newCompanyNumber))
                    return null;

                meterDto = await FindAndValidateMeterDto(device, newCompanyNumber);

                if (meterDto != null)
                    await UpdateDeviceInventoryNumber(_deviceManager, device, newCompanyNumber);
            }

            return meterDto;
        }

        private async Task<MeterDTO> FindAndValidateMeterDto(DeviceInstance device, string inventoryCode)
        {
            var meterDto = await _meterService.FindMeterByInventoryNumber(inventoryCode);
            var isValid = ValidateDeviceWithMeterDto(meterDto, inventoryCode,
                device.Items.SiteInfo.SerialNumber);

            return isValid ? meterDto : null;
        }

        private async Task UpdateDeviceInventoryNumber(IDeviceSessionManager deviceManager, DeviceInstance device,
            string companyNumber)
        {
            var companyNumberItem =
                device.DeviceType.GetItemMetadata<SiteInformationItems>().First(i => i.Number == CompanyNumberItemId);

            var updatedItem = await deviceManager.WriteItemValue(companyNumberItem, companyNumber);
            //var deviceValue = await deviceManager.GetItemValues(new[] {companyNumberItem});

            //if (updatedItem.)
            device.SetItemValues(new[] {updatedItem});
        }

        private bool ValidateDeviceWithMeterDto(MeterDTO meterDto, string companyNumber, string serialNumber)
        {
            if (string.IsNullOrEmpty(meterDto?.InventoryCode)
                || string.IsNullOrEmpty(meterDto?.SerialNumber)
                || meterDto?.InventoryCode != companyNumber
                || meterDto?.SerialNumber.TrimStart('0') != serialNumber)
                return false;

            return true;
        }
    }
}