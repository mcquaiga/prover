namespace Prover.Core.Events
{
    public class LiveReadEvent
    {
        public LiveReadEvent(int itemNumber, decimal liveReadValue)
        {
            LiveReadValue = liveReadValue;
            ItemNumber = itemNumber;
        }

        public decimal LiveReadValue { get; set; }
        public int ItemNumber { get; set; }
    }
}