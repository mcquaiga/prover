using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.ViewModels;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Modules.UnionGas.MasaWebService;

namespace Prover.Modules.UnionGas.VerificationEvents
{
    public class MeterInventoryNumberValidator
    {
        private const string NotFoundMessage = "Couldn't find inventory #{0} on an open job. {1}";

        private const int CompanyNumberItemId = 201;
        private readonly IDeviceSessionManager _deviceManager;
        private readonly IMeterService<MeterDTO> _meterService;

        public MeterInventoryNumberValidator(IDeviceSessionManager deviceManager, IMeterService<MeterDTO> meterService)
        {
            _deviceManager = deviceManager;
            _meterService = meterService;
        }

        public async Task<MeterDTO> Update(DeviceInstance device) =>
            await UpdateInventoryNumber(device, device.CompanyNumber(), device.Items.SiteInfo.SerialNumber);

        public async Task<MeterDTO> Validate(DeviceInstance device, bool updateDeviceItemValue = false)
        {
            var inventoryNumber = device.CompanyNumber();
            var serialNumber = device.Items.SiteInfo.SerialNumber;

            var meterDto = await FindAndValidateMeterDto(serialNumber, inventoryNumber);
            if (meterDto == null)
            {
                if (updateDeviceItemValue)
                    meterDto = await Update(device);
                else
                    await MessageInteractions.ShowMessage.Handle(
                        $"{string.Format(NotFoundMessage, inventoryNumber, Environment.NewLine)}");
            }

            return meterDto;
        }

        private async Task<MeterDTO> FindAndValidateMeterDto(string serialNumber, string inventoryCode)
        {
            var meterDto = await _meterService.FindMeterByInventoryNumber(inventoryCode, serialNumber);
            var isValid = ValidateDeviceWithMeterDto(meterDto, inventoryCode,
                serialNumber);

            return isValid ? meterDto : null;
        }

        private async Task<string> GetInventoryNumberFromUser(string inventoryNumber) =>
            await MessageInteractions.GetInputString.Handle(
                $"{string.Format(NotFoundMessage, inventoryNumber, Environment.NewLine)}" +
                "Enter new inventory number or cancel to continue with");

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

        private async Task<MeterDTO> UpdateInventoryNumber(DeviceInstance device, string inventoryNumber,
            string serialNumber)
        {
            MeterDTO meterDto = null;
            while (meterDto == null)
            {
                inventoryNumber = await GetInventoryNumberFromUser(inventoryNumber);

                if (string.IsNullOrEmpty(inventoryNumber))
                    return null;

                meterDto = await FindAndValidateMeterDto(serialNumber, inventoryNumber);

                if (meterDto != null)
                    await UpdateDeviceInventoryNumber(_deviceManager, device, inventoryNumber);
            }

            return meterDto;
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

    public static class ValidatorEx
    {
        public static async Task<MeterDTO> ValidateInventoryNumber(this MeterInventoryNumberValidator validator,
            EvcVerificationViewModel verification, bool updateDeviceItemValue = false) =>
            await validator.Validate(verification.Device, updateDeviceItemValue);

        public static async Task<MeterDTO> ValidateInventoryNumber(this MeterInventoryNumberValidator validator,
            EvcVerificationTest verification, bool updateDeviceItemValue = false) =>
            await validator.Validate(verification.Device, updateDeviceItemValue);
    }
}