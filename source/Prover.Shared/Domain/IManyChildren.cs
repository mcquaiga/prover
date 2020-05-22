using System.Collections.Generic;

namespace Prover.Shared.Domain
{
	public interface IManyChildren<T> where T : BaseEntity
	{
		//ICollection<T> Children { get; set; }
		//void AddChildren(IEnumerable<T> entities);
		//ICollection<T> SetCollection
		//void AddChildren<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;
		//void AddChild<TEntity>(TEntity entity) where TEntity : BaseEntity;
	}
}