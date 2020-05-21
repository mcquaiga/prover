using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Prover.Application.Models.EvcVerifications;
using Prover.Shared.Extensions;
using static Prover.Application.Services.VerificationFilters;

namespace Prover.Application.Services
{
	public static class VerificationFilters
	{
		public static bool DateTimeFilter(this EvcVerificationTest test, DateTime fromDateTime, DateTime? toDateTime = null)
		{
			var toDate = toDateTime ?? DateTime.Today.AddDays(1);
			return test.TestDateTime.Date.Between(fromDateTime.Date, toDate.Date);
		}

		public static bool ExportedArchivedFilter
				(this EvcVerificationTest test, bool includeExported, bool includeArchived) => (includeArchived || test.IsNotArchived()) || (includeExported || test.ExportedDateTime == null);



		public static Func<EvcVerificationTest, bool> IsVerifiedFilter { get; } = test => test.Verified;
		public static Func<EvcVerificationTest, bool> IsNotVerifiedFilter { get; } = test => !test.Verified;
		public static Func<EvcVerificationTest, bool> IsNotExportedFilter { get; } = test => test.ExportedDateTime == null;



		public static bool IsVerified(this EvcVerificationTest test) => test.Verified;
		public static bool JobAssigned(this EvcVerificationTest test) => test.JobId.IsNotNullOrEmpty();
		public static bool UserAssigned(this EvcVerificationTest test) => test.EmployeeId.IsNotNullOrEmpty();



		public static bool IsExportedOrArchived(this EvcVerificationTest test) => IsExported(test) || IsArchived(test);
		public static bool IsNotArchived(this EvcVerificationTest test) => !test.IsArchived();
		public static bool IsArchived(this EvcVerificationTest test) => test.ArchivedDateTime != null;
		public static bool IsNotExported(this EvcVerificationTest test) => !test.IsExported();
		public static bool IsExported(this EvcVerificationTest test) => test.ExportedDateTime != null;

		public static Task<IEnumerable<EvcVerificationTest>> TestDateBetween(this VerificationQueries query, DateTime fromDateTime, DateTime? toDateTime = null) => query.TestDateBetween(fromDateTime, toDateTime ?? DateTime.Now, false, false);
		public static Task<IEnumerable<EvcVerificationTest>> TestDateBetween(this VerificationQueries query, DateTime fromDateTime, DateTime toDateTime, bool includeArchived, bool includeExported) => query.TestDateBetween(fromDateTime, toDateTime, includeArchived, includeExported);
		public static Task<IEnumerable<EvcVerificationTest>> TestDateBetween(this VerificationQueries query, TimeSpan timeAgoSpan, bool includeArchived = false, bool includeExported = false)
		{
			var toDate = DateTime.Today;
			var fromDate = toDate.Subtract(timeAgoSpan).Date;

			return query.TestDateBetween(fromDate, toDate, includeArchived, includeExported);
		}

		public static Task<IEnumerable<EvcVerificationTest>> TestedLessThanTimeAgo(this VerificationQueries query, TimeSpan timeSpan) => query.TestDateBetween(timeSpan);

		public static Task<IEnumerable<EvcVerificationTest>> TestedLastThirtyDays(this VerificationQueries query) => query.TestDateBetween(TimeSpan.FromDays(30));

		public static Task<IEnumerable<EvcVerificationTest>> TestedDefaultTimeAgo(this VerificationQueries query) => query.TestDateBetween(TimeSpan.FromDays(30));

		internal static Expression<Func<TObj, bool>> IncludePredicate<TObj, TProp>(this Expression<Func<TObj, bool>> predicate, Expression<Func<TObj, TProp>> property, bool include, PredicateBuilder.LogicalType logicalType = PredicateBuilder.LogicalType.And)
		{
			//var prop = property.Compile();
			var exp = IncludePredicate(property, include);

			switch (logicalType)
			{
				case PredicateBuilder.LogicalType.Or:
					return predicate.Or(exp);
				//case PredicateBuilder.LogicalType.And:
				default:
					return predicate.And(exp);
			}
		}

		internal static Expression<Func<TObj, bool>> IncludePredicate<TObj, TProp>(Expression<Func<TObj, TProp>> property, bool include)
		{
			var prop = property.Compile();
			return PredicateBuilder.Create<TObj>(test => include || prop(test) == null);
		}

		public static readonly Dictionary<string, Func<DateTime, bool>> TimeAgoFilters = new Dictionary<string, Func<DateTime, bool>>
		{
				{"1h", time => time.IsLessThanTimeAgo(TimeSpan.FromHours(1))},
				{"1d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(1))},
				{"3d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(3))},
				{"7d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(7))},
				{"30d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(30))}
		};

		public static Func<EvcVerificationTest, bool> BuildDateFilter(string timeKey)
		{
			return test => TimeAgoFilters[timeKey].Invoke(test.TestDateTime);
		}
	}
}