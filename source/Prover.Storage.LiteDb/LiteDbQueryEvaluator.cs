using System.Linq;
using LiteDB;
using Prover.Shared.Domain;
using Prover.Shared.Storage.Interfaces;

namespace Prover.Storage.LiteDb {
	internal class LiteDbQueryEvaluator<T> where T : BaseEntity {
		private static ILiteQueryable<T> _query;
		private static IQuerySpecification<T> _specification;

		public static ILiteQueryable<T> GetQuery(ILiteQueryable<T> inputQuery, IQuerySpecification<T> specification) {
			_query = inputQuery;

			specification.CompileSpecification();
			_specification = specification;

			ApplyPredicate();
			ApplyIncludes();
			ApplyOrderBys();
			ApplyGroupBys();
			ApplyPaging();

			return _query;
		}

		private static void ApplyPredicate() {
			// modify the IQueryable using the specification's criteria expression
			if (_specification.Predicate != null) {
				_query = _query.Where(_specification.Predicate);
			}

		}

		private static void ApplyPaging() {
			// Apply paging if enabled
			//if (_specification is IPagingSpecification pagingSpecification) {
			//	_query = _query.Skip(pagingSpecification.Skip); //.Limit(pagingSpecification.Take);
			//}


		}

		private static void ApplyIncludes() {
			if (_specification is IIncludesSpecification<T> includesSpecification)
				GetIncludes(includesSpecification);

		}

		private static void ApplyGroupBys() {
			//if (specification.GroupBy != null)
			//{
			//	query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
			//}


		}

		private static void ApplyOrderBys() {
			// Apply ordering if expressions are set
			if (_specification.OrderBy != null) {
				_query = _query.OrderBy(_specification.OrderBy);
			}
			else if (_specification.OrderByDescending != null) {
				_query = _query.OrderByDescending(_specification.OrderByDescending);
			}


		}

		private static void GetIncludes(IIncludesSpecification<T> specification) {
			//throw new NotImplementedException("GetIncludes not implemented");
			// Includes all expression-based includes
			//query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

			//// Include any string-based include statements
			//query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

		}
	}
}