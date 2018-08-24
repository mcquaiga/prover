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

        public FrequencyTestItems(IEnumerable<ItemValue> itemValues)
        {
            var items = itemValues.ToList();

            AdjustedVolumeReading = GetHighResolutionValue(items, 850, 851);
            UnadjustVolumeReading = (long)items.GetItem(852).NumericValue;
        }

        public decimal AdjustedVolumeReading { get; set; }
        public long UnadjustVolumeReading { get; set; }
    }
}
