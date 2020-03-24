#region

using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace Prover.Core.Shared.Domain
{
    public abstract class GenericEntity<TId>
    {
        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [Key]
        public TId Id { get; set; }
    }

    /// <summary>
    ///     Entity base class for domain objects with Ids
    /// </summary>
    ///     Id type
    /// </typeparam>
    public abstract class EntityWithId : GenericEntity<Guid>
    {
        protected EntityWithId()
        {
            Id = Guid.NewGuid();
        }

        protected EntityWithId(Guid id)
        {
            Id = id;
        }     

        public virtual void OnInitializing()
        {
        }
    }
}