using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prover.CommProtocol.Common.Items;

namespace Prover.CommProtocol.MiHoneywell.Items
{
    public class FrequencyTestItems : DeviceItems, IFrequencyTestItems
    {    
        [JsonConstructor]
        private FrequencyTestItems() { }

        public FrequencyTestItems(IEnumerable<ItemValue> mainItemValues, IEnumerable<ItemValue> tibItemValues)
        {
            var mainItems = mainItemValues.ToList();

            TibAdjustedVolumeReading = GetHighResolutionValue(tibItemValues.ToList(), 850, 851);
            MainAdjustedVolumeReading = (long)mainItems.GetItem(850).NumericValue;
            UnadjustVolumeReading = (long)mainItems.GetItem(852).NumericValue;
        }

        public decimal TibAdjustedVolumeReading { get; set; }
        public long MainAdjustedVolumeReading { get; set; }
        public long UnadjustVolumeReading { get; set; }
    }
}
