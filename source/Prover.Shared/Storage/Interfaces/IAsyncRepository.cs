using Prover.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;

namespace Prover.Shared.Storage.Interfaces {
	public interface IQueryableRepository<T>
			where T : AggregateRoot {
		Task<int> CountAsync(IQuerySpecification<T> spec);

		IObservable<T> QueryObservable(IQuerySpecification<T> specification);

		Task<IEnumerable<T>> QueryAsync(IQuerySpecification<T> specification);

		Task<IReadOnlyList<T>> ListAsync();
	}

	public interface IAsyncCrudRepository<in TId, TEntity> where TEntity : EntityBase<TId> {

		Task<TEntity> GetAsync(TId id);


		Task<TEntity> UpsertAsync(TEntity entity);


		Task<bool> DeleteAsync(TId id);

	}

	public interface IAsyncRepository<T> : IAsyncCrudRepository<Guid, T>, IQueryableRepository<T>
		where T : AggregateRoot {

	}



	public interface IRequireInitialization {
		IObservable<Unit> Initialized { get; }
		Task Initialize();
	}
}