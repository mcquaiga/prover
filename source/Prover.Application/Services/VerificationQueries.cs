using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Prover.Application.Models.EvcVerifications;
using Prover.Shared.Storage.Interfaces;

namespace Prover.Application.Services
{
	// <summary>    



	public static class VerificationQueriesEx
	{
		public static VerificationQueries Queries(this IAsyncRepository<EvcVerificationTest> repository) => VerificationQueries.Create(repository);
	}

	public class VerificationQueries
	{
		private static readonly VerificationQueries _instance;
		private IAsyncRepository<EvcVerificationTest> _repository;

		static VerificationQueries() => _instance = new VerificationQueries();

		private VerificationQueries()
		{
		}

		private VerificationQueries(IAsyncRepository<EvcVerificationTest> repository) => _repository = repository;

		public IVerificationQueryBuilder Builder => VerificationQueryBuilder.Create();

		public static VerificationQueries Create(IAsyncRepository<EvcVerificationTest> repository) => new VerificationQueries(repository);

		public static VerificationQueries Initialize(IAsyncRepository<EvcVerificationTest> repository)
		{
			_instance._repository = repository;
			return _instance;
		}

		/// <inheritdoc />
		public Task<IEnumerable<EvcVerificationTest>> Query(IQuerySpecification<EvcVerificationTest> spec) => _repository.QueryAsync(spec);

		//public Task<IEnumerable<EvcVerificationTest>> ReadyForExport()
		//{
		//	return Query(test => test.IsExportedOrArchived() == false && test.IsVerified() && test.UserAssigned() && test.JobAssigned());
		//}

		//public Task<IEnumerable<EvcVerificationTest>> TestDateBetween(DateTime fromDateTime, DateTime toDateTime, bool includeArchived, bool includeExported)
		//{
		//	var predicate = Builder.TestedBetween(fromDateTime, toDateTime).IncludeArchived(includeArchived).IncludeExported(includeExported)
		//						   .Build();
		//	return Query(predicate);
		//}
	}
}