using System;
using Devices.Core.Interfaces;
using Domain.Interfaces;
using Domain.Models.EvcVerifications.CorrectionTests;

namespace Domain.Models.EvcVerifications.DriveTypes
{
    public class PulseInputSensor : IDriveType
    {
        public string Discriminator => Drives.PulseInput;

        public PulseInputSensor(IDeviceWithValues device)
        {
            _device = device;
        }

        public int MaxUncorrectedPulses()
        {
            return 10;
        }

        public decimal UnCorrectedInputVolume(decimal appliedInput)
        {
            return 0m;
        }

        private readonly IDeviceWithValues _device;
    }
}