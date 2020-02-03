namespace Devices.Core.Items.DriveTypes
{
    public class RotaryMeterType
    {
        public string Description { get; set; }
        public decimal MeterDisplacement { get; set; }
        public int UnCorPulsesX10 { get; set; }
        public int UnCorPulsesX100 { get; set; }
        public string MountType { get; set; }
    }
}
