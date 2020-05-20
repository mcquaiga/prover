using System;
using System.Linq.Expressions;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Services;
using Prover.Shared.Storage.Interfaces;

namespace Prover.Application.Specifications
{
	public static class VerificationQuerySpecs
	{
		public static IQuerySpecification<EvcVerificationTest> Default { get; } = new VerificationQuerySpecification();
	}

	public class VerificationQuerySpecification : BaseQuerySpecification<EvcVerificationTest>
	{
		private readonly Expression<Func<EvcVerificationTest, bool>> _predicate;

		public VerificationQuerySpecification(Expression<Func<EvcVerificationTest, bool>> predicate)
		{
			_predicate = predicate;
			CompileSpecification();
		}

		public VerificationQuerySpecification()
		{
			CompileSpecification();
		}

		public VerificationQuerySpecification(DateTime fromTestDate, DateTime? toTestDate, bool includeExported = true, bool includeArchived = true)
		{
			FromTestDate = fromTestDate;
			IncludeExported = includeExported;
			IncludeArchived = includeArchived;
			ToTestDate = toTestDate ?? ToTestDate;

			CompileSpecification();
		}

		public bool IncludeArchived { get; set; } = true;
		public bool IncludeExported { get; set; } = true;

		public DateTime FromTestDate { get; set; } = DateTime.Today.AddDays(-30);
		public DateTime ToTestDate { get; set; } = DateTime.Today.AddDays(1);

		/// <inheritdoc />
		public override void CompileSpecification()
		{
			Predicate = _predicate ?? BuildPredicate();
		}

		protected virtual Expression<Func<EvcVerificationTest, bool>> BuildPredicate()
		{
			return PredicateBuilder.Create<EvcVerificationTest>(test =>
					(test.TestDateTime >= FromTestDate && test.TestDateTime <= ToTestDate)
				&& (IncludeExported || test.ExportedDateTime == null)
				&& (IncludeArchived || test.ArchivedDateTime == null)
				);
		}
	}
}