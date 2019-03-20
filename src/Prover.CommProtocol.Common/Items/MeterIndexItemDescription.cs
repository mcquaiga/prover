using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Prover.CommProtocol.Common.Items;

namespace Prover.CommProtocol.Common.Models
{
    public enum RotaryMeterMountType
    {
        B3,
        LMMA,
        RM
    }

    public class MeterIndexItemDescription : ItemMetadata.ItemDescription, IHaveManyId
    {
        public int[] Ids { get; set; }
        public int UnCorPulsesX10 { get; set; }
        public int UnCorPulsesX100 { get; set; }
        public decimal? MeterDisplacement { get; set; }

        [JsonProperty("mountType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public RotaryMeterMountType MountType { get; set; }

        public override string ToString()
        {
            return $"{Description}";
        }
    }
}