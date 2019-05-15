#region

using System;

#endregion

namespace Core.Domain
{
    public abstract class GenericEntity<TId>
    {
        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        public TId Id { get; set; }
    }

    /// <summary>
    ///     Entity base class for domain objects with Ids
    /// </summary>
    ///     Id type
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

        public virtual void OnInitializing()
        {
        }
    }
}