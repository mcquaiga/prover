using System.Collections.Generic;
using Prover.Shared.Domain;

namespace Prover.Shared.Storage
{
    public interface IReadOnlyRepository<TAggregate, TId>
        where TAggregate : AggregateRoot<TId>
    {
        IEnumerable<TAggregate> FindAll();
        TAggregate FindBy(TId id);
    }
}