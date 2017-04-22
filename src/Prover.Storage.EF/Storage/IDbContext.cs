using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Prover.Shared.Domain;

namespace Prover.Storage.EF.Storage
{
    public interface IDbContext
    {
        /// <summary>
        ///     Gets or sets a value indicating whether auto detect changes setting is enabled (used in EF)
        /// </summary>
        bool AutoDetectChangesEnabled { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether proxy creation setting is enabled (used in EF)
        /// </summary>
        bool ProxyCreationEnabled { get; set; }

        /// <summary>
        ///     Detach an entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Detach(object entity);

        /// <summary>
        ///     Executes the given DDL/DML command against the database.
        /// </summary>
        /// <param name="sql">The command string</param>
        /// <param name="doNotEnsureTransaction">
        ///     false - the transaction creation is not ensured; true - the transaction creation
        ///     is ensured.
        /// </param>
        /// <param name="timeout">
        ///     Timeout value, in seconds. A null value indicates that the default value of the underlying
        ///     provider will be used
        /// </param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>The result returned by the database after executing the command.</returns>
        Task<int> ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null,
            params object[] parameters);

        /// <summary>
        ///     Execute stores procedure and load a list of entities at the end
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="commandText">Command text</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Entities</returns>
        IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters)
            where TEntity : Entity, new();

        /// <summary>
        ///     Save changes
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        ///     Get DbSet
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>DbSet</returns>
        DbSet<TEntity> Set<TEntity>() where TEntity : Entity;

        /// <summary>
        ///     Creates a raw SQL query that will return elements of the given generic type.  The type can be any type that has
        ///     properties that match the names of the columns returned from the query, or can be a simple primitive type. The type
        ///     does not have to be an entity type. The results of this query are never tracked by the context even if the type of
        ///     object returned is an entity type.
        /// </summary>
        /// <typeparam name="TElement">The type of object returned by the query.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>Result</returns>
        Task<int> SqlQuery<TElement>(string sql, params object[] parameters);
    }
}