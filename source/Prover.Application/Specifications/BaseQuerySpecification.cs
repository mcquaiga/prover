using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Prover.Application.Helpers.Query;
using Prover.Shared.Storage.Interfaces;

namespace Prover.Application.Specifications
{
	public abstract class BaseQuerySpecification<T> : IQuerySpecification<T>
	{
		protected BaseQuerySpecification(Expression<Func<T, bool>> criteria)
		{
			Predicate = criteria;
		}

		protected BaseQuerySpecification()
		{

		}
		#region Public Properties

		public Expression<Func<T, bool>> Predicate { get; protected set; }
		public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
		public List<string> IncludeStrings { get; } = new List<string>();
		public bool IsPagingEnabled { get; private set; }
		public Expression<Func<T, object>> OrderBy { get; private set; }
		public Expression<Func<T, object>> OrderByDescending { get; private set; }

		/// <inheritdoc />
		public virtual void CompileSpecification()
		{

		}

		public Expression<Func<T, object>> GroupBy { get; private set; }

		public int Take { get; private set; }
		public int Skip { get; private set; }

		#endregion

		#region Protected

		protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
		{
			Includes.Add(includeExpression);
		}

		protected virtual void AddInclude(string includeString)
		{
			IncludeStrings.Add(includeString);
		}

		protected virtual void AddIncludes<TProperty>(
			Func<IncludeAggregator<T>, IIncludeQuery<T, TProperty>> includeGenerator)
		{
			var includeQuery = includeGenerator(new IncludeAggregator<T>());
			IncludeStrings.AddRange(includeQuery.Paths);
		}

		//Not used anywhere at the moment, but someone requested an example of setting this up.
		protected virtual void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
		{
			GroupBy = groupByExpression;
		}

		protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
		{
			OrderBy = orderByExpression;
		}

		protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
		{
			OrderByDescending = orderByDescendingExpression;
		}

		protected virtual void ApplyPaging(int skip, int take)
		{
			Skip = skip;
			Take = take;
			IsPagingEnabled = true;
		}

		#endregion
	}
}