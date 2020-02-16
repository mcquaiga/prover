#region

using System;

#endregion

namespace Shared.Domain
{
    public interface IEntity
    {
    }

    public abstract class GenericEntity<TId> : IEntity
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        public TId Id { get; protected set; }

        #endregion
    }


    /// <summary>
    ///     Entity base class for domain objects with Ids
    /// </summary>
    /// Id type
    /// </typeparam>
    public abstract class BaseEntity : GenericEntity<Guid>
    {
        protected BaseEntity()
        {
            Id = Guid.NewGuid();
        }

        protected BaseEntity(Guid id)
        {
            Id = id;
        }

        #region Public Methods

        public virtual void OnInitializing()
        {
        }

        #endregion
    }
}