using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Shared.Domain;
using Prover.Shared.Storage.Interfaces;

namespace Prover.Application.Caching
{
	public interface ICachedRepository
	{
		IObservable<Unit> StartAsync(CancellationToken cancellationToken);
		Task StopAsync(CancellationToken cancellationToken);
	}

	//public interface IEntityRepository<T> : IAsyncRepository<T>, ICacheAggregateRoot<T> where T : AggregateRoot
	//{

	//}
}