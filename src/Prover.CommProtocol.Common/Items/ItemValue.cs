namespace Prover.CommProtocol.Common.Items
{
    public class ItemValue
    {
        public ItemValue(ItemMetadata metadata, string value)
        {
            RawValue = value;
            Metadata = metadata;
        }

        protected string RawValue { get; private set; }
        public ItemMetadata Metadata { get; private set; }
    }
}