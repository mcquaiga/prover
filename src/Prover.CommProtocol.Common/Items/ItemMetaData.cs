using System.Collections.Generic;

namespace Prover.CommProtocol.Common.Items
{
    public class ItemMetadata
    {
        public string Code { get; set; }

        public bool? IsAlarm { get; set; }
        public bool? IsPressure { get; set; }
        public bool? IsPressureTest { get; set; }
        public bool? IsSuperFactor { get; set; }
        public bool? IsTemperature { get; set; }
        public bool? IsTemperatureTest { get; set; }
        public bool? IsVolume { get; set; }
        public bool? IsVolumeTest { get; set; }

        public virtual IEnumerable<ItemDescription> ItemDescriptions { get; set; }
        public string LongDescription { get; set; }
        public int Number { get; set; }
        public string ShortDescription { get; set; }

        public class ItemDescription
        {
            public string Description { get; set; } //Human displayed description
            public int Id { get; set; } //Maps to the Id that the instrument uses
            public double? Value { get; set; } // Numeric value used for calculations, etc.
        }
    }
}