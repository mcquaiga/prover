using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Devices.Honeywell.Core.Items
{
    public class FrequencyTestItems : DeviceItems, IFrequencyTestItems
    {
        #region Constructors

        public FrequencyTestItems(IEnumerable<ItemValue> mainItemValues, IEnumerable<ItemValue> tibItemValues)
        {
            var mainItems = mainItemValues.ToList();

            TibAdjustedVolumeReading = GetHighResolutionValue(tibItemValues.ToList(), 850, 851);
            TibUnadjustedVolumeReading = (long)tibItemValues.GetItem(852).NumericValue;

            MainAdjustedVolumeReading = (long)mainItems.GetItem(850).NumericValue;
            MainUnadjustVolumeReading = (long)mainItems.GetItem(852).NumericValue;
        }

        #endregion

        #region Properties

        public long MainAdjustedVolumeReading { get; set; }

        public long MainUnadjustVolumeReading { get; set; }

        public decimal TibAdjustedVolumeReading { get; set; }

        public long TibUnadjustedVolumeReading { get; }

        #endregion

        [JsonConstructor]
        private FrequencyTestItems() { }
    }
}