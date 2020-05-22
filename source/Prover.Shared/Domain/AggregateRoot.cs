#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace Prover.Shared.Domain
{
	public abstract class AggregateRoot : BaseEntity, IAggregateRoot
	{
		private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

		protected AggregateRoot()
		{
		}

		protected AggregateRoot(Guid id) : base(id)
		{
		}

		public virtual IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

		public DateTime Created { get; set; } = DateTime.UtcNow;

		public DateTime? LastUpdated { get; set; }

		public DateTime? Archived { get; set; }

		public virtual void AddDomainEvent(IDomainEvent newEvent)
		{
			_domainEvents.Add(newEvent);
		}

		public virtual void ClearEvents()
		{
			_domainEvents.Clear();
		}
	}

	public abstract class AggregateRoot<T> : AggregateRoot where T : class
	{
		private readonly Dictionary<Type, object> _childrenDicts = new Dictionary<Type, object>();

		protected AggregateRoot(ICollection<T> children)
		{
			//_children = children;
		}

		protected AggregateRoot()
		{
			//Children = new List<T>();
			//SetupManyProperty();
		}

		private void SetupManyProperty()
		{
			GetType().GetTypeInfo().ImplementedInterfaces.Where(i => i.IsGenericType && i.IsTypeOf(typeof(IManyChildren<>))).ToList();
		}

		protected abstract ICollection<T> Children { get; }

		public virtual void AddChild(T entity)
		{
			if (!Children.Contains(entity))
				Children.Add(entity);
		}

		public virtual void AddChildren(IEnumerable<T> entities)
		{
			Children.AddRangeIfNotContains(entities.ToArray());
		}

		//public virtual void AddChildren<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
		//{
		//	if (!_childrenDicts.ContainsKey(typeof(TEntity)))
		//		SetupChildCollection<TEntity>(new List<TEntity>());

		//	var col = (_childrenDicts[typeof(TEntity)] as ICollection<TEntity>);
		//	entities.ForEach(col.Add);
		//}

		//public virtual void AddChild<TEntity>(TEntity entity) where TEntity : BaseEntity
		//{
		//	AddChildren<TEntity>(new[] { entity });
		//}

		protected void SetupChildCollection<TEntity>(ICollection<TEntity> collection) where TEntity : BaseEntity
		{
			if (!_childrenDicts.ContainsKey(typeof(TEntity)))
				_childrenDicts.Add(typeof(TEntity), collection);
		}

		//protected abstract void SetChildCollection(ICollection<T> children);
	}

	public static class AggregateRootMixins
	{
		public static void AddChildren<T>(this AggregateRoot<T> root, IEnumerable<T> entities) where T : BaseEntity
		{
			root.AddChildren(entities);
		}
	}
}