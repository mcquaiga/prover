using System;
using System.Threading.Tasks;
using DynamicData;
using Prover.Domain.EvcVerifications;
using Prover.Shared.Domain;

namespace Prover.Application.Interfaces
{
    public interface ICacheManager
    {
        Task LoadAsync();
        void Update();
    }

    public interface IEntityDataCache<TEntity>
            where TEntity : AggregateRoot
    {
        IObservableCache<TEntity, Guid> Updates { get; }
        IObservableList<TEntity> Data(Func<EvcVerificationTest, bool> filter = null);
    }
}