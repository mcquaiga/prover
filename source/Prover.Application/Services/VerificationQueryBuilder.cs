using System;
using System.Linq.Expressions;
using Prover.Application.Models.EvcVerifications;

namespace Prover.Application.Services
{
	public interface IVerificationQueryBuilder
	{
		VerificationQueryBuilder TestedBetween(DateTime fromDateTime, DateTime? toDateTime = null);
	}

	public class VerificationQueryBuilder : IVerificationQueryBuilder
	{
		private Expression<Func<EvcVerificationTest, bool>> _includeArchived;
		private Expression<Func<EvcVerificationTest, bool>> _includeExported;
		private Expression<Func<EvcVerificationTest, bool>> _predicate;

		private VerificationQueryBuilder()
		{
			IncludeExported(false);
			IncludeArchived(false);
		}

		public Expression<Func<EvcVerificationTest, bool>> Build()
		{
			_predicate = _predicate.And(_includeArchived.Or(_includeExported));
			return _predicate;
		}

		public VerificationQueryBuilder IncludeArchived(bool include = true)
		{
			_includeArchived = Include(t => t.ArchivedDateTime, include);
			return this;
		}

		public VerificationQueryBuilder IncludeExported(bool include = true)
		{
			_includeExported = Include(t => t.ExportedDateTime, include);
			return this;
		}

		public VerificationQueryBuilder TestedBetween(DateTime fromDateTime, DateTime? toDateTime = null)
		{
			var toDate = toDateTime ?? DateTime.Today;
			var exp = PredicateBuilder.Create<EvcVerificationTest>(test => test.TestDateTime.Date >= fromDateTime && test.TestDateTime.Date <= toDate);

			_predicate = _predicate?.And(exp) ?? exp;

			return this;
		}

		internal static IVerificationQueryBuilder Create() => new VerificationQueryBuilder();

		private Expression<Func<EvcVerificationTest, bool>> Include<TProp>
				(Expression<Func<EvcVerificationTest, TProp>> property, bool include) => VerificationFilters.IncludePredicate(property, include);
	}
}