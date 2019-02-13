using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prover.CommProtocol.Common.Items;

namespace Prover.CommProtocol.MiHoneywell.Items
{
    internal class FrequencyTestItems : DeviceItems, IFrequencyTestItems
    {    
        [JsonConstructor]
        private FrequencyTestItems() { }

        public FrequencyTestItems(IEnumerable<ItemValue> mainItemValues, IEnumerable<ItemValue> tibItemValues)
        {
            var mainItems = mainItemValues.ToList();

            TibAdjustedVolumeReading = GetHighResolutionValue(tibItemValues.ToList(), 850, 851);
            TibUnadjustedVolumeReading = (long)tibItemValues.GetItem(852).NumericValue;
            MainAdjustedVolumeReading = (long)mainItems.GetItem(850).NumericValue;
            MainUnadjustVolumeReading = (long)mainItems.GetItem(852).NumericValue;
        }

        public decimal TibAdjustedVolumeReading { get; set; }
        public long TibUnadjustedVolumeReading { get; }
        public long MainAdjustedVolumeReading { get; set; }
        public long MainUnadjustVolumeReading { get; set; }
    }
}
