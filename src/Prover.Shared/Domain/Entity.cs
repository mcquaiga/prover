// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Entity.cs" company="">
//   
// </copyright>
// <summary>
//   Entity base class for domain objects
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Prover.Shared.Domain
{
    /// <summary>
    ///     Entity base class for domain objects
    /// </summary>
    /// <typeparam name="TId">
    ///     Id type
    /// </typeparam>
    public abstract class Entity<TId>
    {
        protected Entity(TId id)
        {
            Id = id;
        }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        public TId Id { get; set; }

        /// <summary>
        ///     The ==.
        /// </summary>
        /// <param name="entity1">
        ///     The entity 1.
        /// </param>
        /// <param name="entity2">
        ///     The entity 2.
        /// </param>
        /// <returns>
        ///     True if the entity are equal
        /// </returns>
        public static bool operator ==(Entity<TId> entity1, Entity<TId> entity2)
        {
            if ((object) entity1 == null && (object) entity2 == null)
                return true;

            if ((object) entity1 == null || (object) entity2 == null)
                return false;

            if (entity1.Id.ToString() == entity2.Id.ToString())
                return true;

            return false;
        }

        /// <summary>
        ///     The !=.
        /// </summary>
        /// <param name="entity1">
        ///     The entity 1.
        /// </param>
        /// <param name="entity2">
        ///     The entity 2.
        /// </param>
        /// <returns>
        ///     Returns true is they are not equal
        /// </returns>
        public static bool operator !=(Entity<TId> entity1, Entity<TId> entity2)
        {
            return !(entity1 == entity2);
        }

        /// <summary>
        ///     The equals.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public override bool Equals(object entity)
        {
            return entity != null
                   && entity is Entity<TId>
                   && this == (Entity<TId>) entity;
        }

        /// <summary>
        ///     The get hash code.
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}