using Devices.Core.Interfaces;
using Domain.Interfaces;

namespace Domain.Models.EvcVerifications.DriveTypes
{
    public class RotaryDrive : IDriveType, IAssertPassFail
    {
        public string Discriminator => Drives.Rotary;
        public MeterTest Meter { get; set; }
        public bool Passed => Meter.Passed;

        public RotaryDrive(IDeviceWithValues device)
        {
            _device = device;
            Meter = new MeterTest(_device);
        }

        public int MaxUncorrectedPulses()
        {
            if (_device.Volume.UncorrectedMultiplier == 10)
                return Meter.MeterIndex.UnCorPulsesX10;

            if (_device.Volume.UncorrectedMultiplier == 100)
                return Meter.MeterIndex.UnCorPulsesX100;

            return 10; //Low standard number if we can't find anything
        }

        public decimal UnCorrectedInputVolume(decimal appliedInput)
        {
            return Meter.MeterDisplacement * appliedInput;
        }

        private readonly IDeviceWithValues _device;
    }
}