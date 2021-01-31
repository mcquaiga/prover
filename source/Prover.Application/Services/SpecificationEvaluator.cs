using System;
using System.Linq;
using Prover.Shared.Domain;
using Prover.Shared.Storage.Interfaces;

namespace Prover.Application.Services {
	public class SpecificationEvaluator<T> where T : EntityBase {
		public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, IQuerySpecification<T> specification) {
			var query = inputQuery;

			specification.CompileSpecification();

			query = ApplyPredicate(specification, query);
			query = ApplyIncludes(specification, query);
			query = ApplyOrderBys(specification, query);
			query = ApplyGroupBys(specification, query);
			query = ApplyPaging(specification, query);

			return query;
		}

		private static IQueryable<T> ApplyPredicate(IQuerySpecification<T> specification, IQueryable<T> query) {
			// modify the IQueryable using the specification's criteria expression
			if (specification.Predicate != null) {
				query = query.Where(specification.Predicate);
			}

			return query;
		}

		private static IQueryable<T> ApplyPaging(IQuerySpecification<T> specification, IQueryable<T> query) {
			// Apply paging if enabled
			if (specification is IPagingSpecification pagingSpecification) {
				query = query.Skip(pagingSpecification.Skip).Take(pagingSpecification.Take);
			}

			return query;
		}

		private static IQueryable<T> ApplyIncludes(IQuerySpecification<T> specification, IQueryable<T> query) {
			if (specification is IIncludesSpecification<T> includesSpecification)
				query = GetIncludes(includesSpecification, query);
			return query;
		}

		private static IQueryable<T> ApplyGroupBys(IQuerySpecification<T> specification, IQueryable<T> query) {
			//if (specification.GroupBy != null)
			//{
			//	query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
			//}

			return query;
		}

		private static IQueryable<T> ApplyOrderBys(IQuerySpecification<T> specification, IQueryable<T> query) {
			// Apply ordering if expressions are set
			if (specification.OrderBy != null) {
				query = query.OrderBy(specification.OrderBy);
			}
			else if (specification.OrderByDescending != null) {
				query = query.OrderByDescending(specification.OrderByDescending);
			}

			return query;
		}

		private static IQueryable<T> GetIncludes(IIncludesSpecification<T> specification, IQueryable<T> query) {
			//throw new NotImplementedException("GetIncludes not implemented");
			// Includes all expression-based includes
			//query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

			//// Include any string-based include statements
			//query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));
			return query;
		}
	}
}