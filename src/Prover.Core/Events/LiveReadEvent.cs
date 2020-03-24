using Prover.CommProtocol.Common.Items;

namespace Prover.Core.Events
{
    public class LiveReadEvent
    {
        public LiveReadEvent(ItemMetadata item, decimal liveReadValue)
        {
            LiveReadValue = liveReadValue;
            Item = item;
        }

        public decimal LiveReadValue { get; set; }
        public ItemMetadata Item { get; set; }
    }
}