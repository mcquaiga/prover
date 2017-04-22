using System.Collections.Generic;

namespace Prover.Shared.Domain
{
    public interface IAggregateRoot
    {
        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        void AddDomainEvent(IDomainEvent newEvent);
        void ClearEvents();
    }
}