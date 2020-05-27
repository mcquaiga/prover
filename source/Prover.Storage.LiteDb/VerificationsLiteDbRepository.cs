using Devices.Core.Repository;
using LiteDB;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Services;
using Prover.Shared.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;

namespace Prover.Storage.LiteDb {
	public class VerificationsLiteDbRepository : BaseLiteDbAsyncRepository<Guid, EvcVerificationTest>, IAsyncRepository<EvcVerificationTest> {

		public VerificationsLiteDbRepository(ILiteDatabase context) : base(context) {

		}

		/// <inheritdoc />
		public Task<int> CountAsync(IQuerySpecification<EvcVerificationTest> spec) => throw new NotImplementedException();

		/// <inheritdoc />
		public IObservable<EvcVerificationTest> QueryObservable(IQuerySpecification<EvcVerificationTest> specification) {
			return Observable.StartAsync(async () => await QueryAsync(specification))
							 .SelectMany(t => t);

		}

		/// <inheritdoc />
		public Task<IEnumerable<EvcVerificationTest>> QueryAsync(IQuerySpecification<EvcVerificationTest> specification) {

			return Task.FromResult(
								LiteDbQueryEvaluator<EvcVerificationTest>.GetQuery(Collection.Query(), specification)
																		 .ToEnumerable()
							);

		}

		public Task<IReadOnlyList<EvcVerificationTest>> ListAsync() {
			return Task.FromResult(
						(IReadOnlyList<EvcVerificationTest>)Collection.FindAll().ToList());
		}

		///// <inheritdoc />
		//public async Task<IEnumerable<EvcVerificationTest>> Query(Expression<Func<T, bool>> predicate = null)
		//{
		//	predicate = null;
		//	var results = predicate != null ? Collection.Find(predicate) : Context.GetCollection<T>().FindAll();
		//	return await Task.FromResult(results);
		//}

		/// <inheritdoc />
		//public Task<IEnumerable<T>> Query(IQuerySpecification<T> specification) => throw new NotImplementedException();

		///// <inheritdoc />
		//public IObservable<T> QueryObservable(IQuerySpecification<T> specification) => throw new NotImplementedException();

		//public Task<int> CountAsync(IQuerySpecification<T> spec) => throw new NotImplementedException();

		//private IQueryable<EvcVerificationTest> ApplySpecification(IQuerySpecification<EvcVerificationTest> spec)
		//{
		//	//(IQueryable<EvcVerificationTest>)
		//	return 
		//}

		//private IQueryable<T> ApplySpecification(IQuerySpecification<T> spec)
		//{
		//	return ApplySpecification(_container.GetItemLinqQueryable<T>(allowSynchronousQueryExecution: true), spec);
		//}
	}
}