using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentModelBuilder.Configuration;
using FluentModelBuilder.Extensions;
using Microsoft.EntityFrameworkCore;
using Prover.Domain.Model.Verifications;
using Prover.Shared.Domain;
using Prover.Storage.EF.Mappers;
using Prover.Storage.EF.Storage;

namespace Prover.Storage.EF
{
    public class EfProverContext : DbContext, IDbContext
    {
        private readonly string SqliteFilePath = @"Data Source = prover_data.db";
        const string SqliteInMemoryConnection = "DataSource=:memory:";

        public EfProverContext(DbContextOptions options) : base(options)
        {
        }

        public bool AutoDetectChangesEnabled { get; set; }
        public bool ProxyCreationEnabled { get; set; }


        public void Detach(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Entry(entity).State = EntityState.Detached;
        }

        public async Task<int> ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null,
            params object[] parameters)
        {
            return await Database.ExecuteSqlCommandAsync(sql, new CancellationToken(), parameters);
        }

        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters)
            where TEntity : Entity, new()
        {
            throw new NotImplementedException();
        }

        public new DbSet<TEntity> Set<TEntity>() where TEntity : Entity
        {
            return base.Set<TEntity>();
        }

        public async Task<int> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return await Database.ExecuteSqlCommandAsync(sql, new CancellationToken(), parameters);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlite(SqliteFilePath);
                
            optionsBuilder.Configure(
                From.AssemblyOf<VerificationRun>(new EntityAutoConfiguration())
                    .UseOverridesFromAssemblyOf<EfProverContext>());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    // Pick up all concrete types that inherit from Entity
}