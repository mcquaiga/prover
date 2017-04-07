using System.Collections.Generic;

namespace Prover.Shared.Domain
{
    public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
    {
        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

        protected AggregateRoot(TId id) : base(id)
        {
        }

        public virtual IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

        public virtual void AddDomainEvent(IDomainEvent newEvent)
        {
            _domainEvents.Add(newEvent);
        }

        public virtual void ClearEvents()
        {
            _domainEvents.Clear();
        }
    }
}