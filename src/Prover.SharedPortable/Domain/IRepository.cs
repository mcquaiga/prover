namespace Prover.Shared.Domain
{
    public interface IRepository<TAggregate, TId> : IReadOnlyRepository<TAggregate, TId> 
        where TAggregate : AggregateRoot<TId>
    {
        void Delete(TAggregate aggregate);
        void Insert(TAggregate aggregate);
        void Update(TAggregate aggregate);
    }
}