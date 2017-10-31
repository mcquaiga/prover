using Prover.CommProtocol.Common.Items;

namespace Prover.CommProtocol.Common.Models
{
    public class MeterIndexItemDescription : ItemMetadata.ItemDescription
    {            
        public int[] Ids { get; set; }        
        public int UnCorPulsesX10 { get; set; }
        public int UnCorPulsesX100 { get; set; }
        public decimal? MeterDisplacement { get; set; }
    }
}