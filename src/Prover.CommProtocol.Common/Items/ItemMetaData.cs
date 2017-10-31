using System.Collections.Generic;

namespace Prover.CommProtocol.Common.Items
{
    public class ItemGroup
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public bool? IsAlarm { get; set; }
        public bool? IsPressure { get; set; }
        public bool? IsPressureTest { get; set; }
        public bool? IsTemperature { get; set; }
        public bool? IsVolume { get; set; }
        public bool? IsVolumeTest { get; set; }
        public bool? IsSuperFactor { get; set; }
    }

    public class ItemMetadata
    {
        public int Number { get; set; }
        public string Code { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }

        public bool? IsAlarm { get; set; } = false;
        public bool? IsPressure { get; set; } = false;
        public bool? IsPressureTest { get; set; } = false;
        public bool? IsTemperature { get; set; } = false;
        public bool? IsTemperatureTest { get; set; } = false;
        public bool? IsVolume { get; set; } = false;
        public bool? IsVolumeTest { get; set; } = false;
        public bool? IsSuperFactor { get; set; } = false;

        public virtual IEnumerable<ItemDescription> ItemDescriptions { get; set; }

        public class ItemDescription
        {
            public int Id { get; set; } //Maps to the Id that the instrument uses
            public string Description { get; set; } //Human displayed description
            public decimal? NumericValue { get; set; } // Numeric value used for calculations, etc.
        }
    }
}