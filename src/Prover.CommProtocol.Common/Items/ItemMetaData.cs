using System.Collections.Generic;
using System.Linq;

namespace Prover.CommProtocol.Common.Items
{
    public interface IHaveOneId
    {
        int Id { get; set; }
    }

    public interface IHaveManyId
    {
        int[] Ids { get; set; }
    }

    public class ItemGroup
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public bool? IsAlarm { get; set; }
        public bool? IsPressure { get; set; }
        public bool? IsPressureTest { get; set; }
        public bool? IsTemperature { get; set; }
        public bool? IsTemperatureTest { get; set; }      
        public bool? IsSuperFactor { get; set; }
        public bool? IsVolume { get; set; }
        public bool? IsVolumeTest { get; set; }
        public bool? IsFrequencyTest { get; set; }
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
        public bool? IsFrequencyTest { get; set; } = false;

        public bool? IsLiveReadPressure { get; set; }
        public bool? IsLiveReadTemperature { get; set; }

        public bool CanVerify { get; set; } = true;
        public bool CanReset { get; set; } = true;

        public virtual IEnumerable<ItemDescription> ItemDescriptions { get; set; }        
        public virtual ItemDescription GetItemDescription(string rawValue)
        {
                if (ItemDescriptions != null && ItemDescriptions.Any())
                {
                    if (!int.TryParse(rawValue.Trim(), out var intValue)) return null;

                    var result = ItemDescriptions.FirstOrDefault(x
                        => (x as IHaveManyId)?.Ids.Contains(intValue) ?? false);

                    if (result == null)
                        result = ItemDescriptions.FirstOrDefault(x => (x as IHaveOneId)?.Id == intValue);

                    return result;
                }

                return null;
        }

        public abstract class ItemDescriptionBase
        {
            public string Description { get; set; } //Human displayed description
            public decimal? NumericValue { get; set; } // Numeric value used for calculations, etc.
        }

        public class ItemDescription : ItemDescriptionBase, IHaveOneId
        {
            public int Id { get; set; } //Maps to the Id that the instrument uses           
        }
        
    }
}