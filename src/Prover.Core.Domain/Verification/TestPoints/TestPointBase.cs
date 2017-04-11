using System;
using Prover.Domain.Instrument.Items;
using Prover.Shared.Domain;

namespace Prover.Domain.Verification.TestPoints
{
    public abstract class TestPointBase<TItems> : Entity<Guid>
        where TItems : IItemsGroup
    {
        protected TestPointBase(Guid id) : base(id)
        {
            EvcItems = default(TItems);
        }

        protected TestPointBase(Guid id, TItems evcItems) : base(id)
        {
            EvcItems = evcItems;
        }

        public TItems EvcItems { get; set; }
    }
}