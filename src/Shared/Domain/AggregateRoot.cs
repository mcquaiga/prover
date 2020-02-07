#region

using System;
using System.Collections.Generic;

#endregion

namespace Shared.Domain
{
    public abstract class AggregateRoot : BaseEntity, IAggregateRoot
    {
        public virtual IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

        public virtual void AddDomainEvent(IDomainEvent newEvent)
        {
            _domainEvents.Add(newEvent);
        }

        public virtual void ClearEvents()
        {
            _domainEvents.Clear();
        }

        protected AggregateRoot() : base()
        {
        }

        protected AggregateRoot(Guid id) : base(id)
        {
        }

        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
    }
}