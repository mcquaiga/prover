#region

using System.Collections.Generic;

#endregion

namespace Prover.Core.Shared.Domain
{
    public interface IAggregateRoot
    {
        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        void AddDomainEvent(IDomainEvent newEvent);
        void ClearEvents();
    }
}