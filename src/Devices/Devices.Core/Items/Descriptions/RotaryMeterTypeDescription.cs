using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Shared;

namespace Devices.Core.Items.Descriptions
{
    public class RotaryMeterTypeDescription : ItemDescription, IHaveManyId
    {
        #region Public Properties

        public int[] Ids { get; set; }
        public decimal? MeterDisplacement { get; set; }

        [JsonProperty("mountType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public RotaryMeterMountType MountType { get; set; }

        public int UnCorPulsesX10 { get; set; }
        public int UnCorPulsesX100 { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return $"{Description}";
        }

        #endregion Public Methods
    }
}