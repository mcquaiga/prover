using DynamicData;
using Prover.Application.Models.EvcVerifications;
using Prover.Shared.Domain;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Prover.Shared.Storage.Interfaces;

namespace Prover.Application.Interfaces
{
	public interface IEntityCache<T>
		where T : AggregateRoot
	{
		IObservableCache<T, Guid> Data { get; }
		Task Refresh(IQuerySpecification<EvcVerificationTest> specification = null);
	}

	public interface ICacheClient<T> : IEntityCache<T>
			where T : AggregateRoot
	{
		Task LoadAsync(IObservable<T> entityObservable);

		Task SetAsync(T entity);

		Task<T> GetAsync(Guid id);
	}
}