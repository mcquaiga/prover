using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Devices.Honeywell.Core.ItemGroups
{
    internal class FrequencyItems : ItemGroupBase, IFrequencyTestItems
    {
        public long MainAdjustedVolumeReading { get; set; }

        public long MainUnadjustVolumeReading { get; set; }

        public decimal TibAdjustedVolumeReading { get; set; }

        public long TibUnadjustedVolumeReading { get; }

        public FrequencyItems(IEnumerable<ItemValue> mainItemValues, IEnumerable<ItemValue> tibItemValues)
        {
            var mainItems = mainItemValues.ToList();

            //TibAdjustedVolumeReading = GetHighResolutionValue(tibItemValues.ToList(), 850, 851);
            //TibUnadjustedVolumeReading = (long)tibItemValues.GetItem(852).NumericValue;

            //MainAdjustedVolumeReading = (long)mainItems.GetItem(850).NumericValue;
            //MainUnadjustVolumeReading = (long)mainItems.GetItem(852).NumericValue;
        }

        [JsonConstructor]
        private FrequencyItems() { }
    }
}