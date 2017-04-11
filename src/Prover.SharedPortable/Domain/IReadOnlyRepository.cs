using System.Collections.Generic;

namespace Prover.Shared.Domain
{
    public interface IReadOnlyRepository<TAggregate, TId>
        where TAggregate : AggregateRoot<TId>
    {
        IEnumerable<TAggregate> FindAll();
        TAggregate FindBy(TId id);
    }
}