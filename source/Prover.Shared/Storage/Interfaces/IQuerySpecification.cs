using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Prover.Shared.Storage.Interfaces
{

	public interface IQuerySpecification<T>
	{
		#region Properties

		Expression<Func<T, bool>> Predicate { get; }

		Expression<Func<T, object>> OrderBy { get; }

		Expression<Func<T, object>> OrderByDescending { get; }

		void CompileSpecification();

		#endregion
	}

	public interface IPagingSpecification
	{
		bool IsPagingEnabled { get; }

		int Skip { get; }

		int Take { get; }
	}

	public interface IIncludesSpecification<T>
	{
		List<Expression<Func<T, object>>> Includes { get; }

		List<string> IncludeStrings { get; }
	}


}