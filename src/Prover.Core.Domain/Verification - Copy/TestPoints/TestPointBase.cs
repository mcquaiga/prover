namespace Prover.Domain.Verification.TestPoints
{
    public abstract class TestPointBase<TItems> : Entity<Guid>
        where TItems : IItemsGroup
    {
        protected TestPointBase(Guid id) : base((Guid) id)
        {
            EvcItems = default(TItems);
        }

        protected TestPointBase(Guid id, TItems evcItems) : base((Guid) id)
        {
            EvcItems = evcItems;
        }

        public TItems EvcItems { get; set; }
    }
}