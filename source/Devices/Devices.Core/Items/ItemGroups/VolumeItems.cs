using Prover.Shared;

namespace Devices.Core.Items.ItemGroups
{
    public class VolumeItems : ItemGroup
    {
        public virtual decimal CorrectedMultiplier { get; set; }
        public virtual decimal CorrectedReading { get; set; }
        public virtual string CorrectedUnits { get; set; }
        public virtual decimal UncorrectedMultiplier { get; set; }
        public virtual decimal UncorrectedReading { get; set; }
        public virtual string UncorrectedUnits { get; set; }
        public virtual VolumeInputType VolumeInputType { get; set; }
        public virtual decimal DriveRate { get; set; }
        public virtual string DriveRateDescription { get; set; }
    }
}