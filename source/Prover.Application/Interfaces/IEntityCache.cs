using DynamicData;
using Prover.Application.Models.EvcVerifications;
using Prover.Shared.Domain;
using System;
using System.Linq.Expressions;

namespace Prover.Application.Interfaces
{
    public interface IEntityDataCache<TEntity> : IDisposable
            where TEntity : AggregateRoot
    {
        IObservableCache<TEntity, Guid> Items { get; }
        //IObservableList<TEntity> Data { get; }
        void Update(Expression<Func<EvcVerificationTest, bool>> filter = null);
        void ApplyDateFilter(string dateTimeKey);
        IObservableCache<EvcVerificationTest, Guid> LoadCache(Expression<Func<EvcVerificationTest, bool>> filter);
    }
}