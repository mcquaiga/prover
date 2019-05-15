#region

using System.Collections.Generic;

#endregion

namespace Domain.Entities
{
    public interface IAggregateRoot
    {
        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        void AddDomainEvent(IDomainEvent newEvent);
        void ClearEvents();
    }
}