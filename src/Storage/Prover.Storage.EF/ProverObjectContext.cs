using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using FluentModelBuilder.Configuration;
using FluentModelBuilder.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Prover.Domain.Model.Verifications;
using Prover.Shared.Domain;
using Prover.Storage.EF.Storage;

namespace Prover.Storage.EF
{
    public static class DataContextFactory
    {
        public static ProverObjectContext CreateWithSqlite(string connString)
        {
            var connection = new SqliteConnection(connString);
            connection.Open();

            var options = new DbContextOptionsBuilder<ProverObjectContext>()
                .UseSqlite(connection)
                .Options;

            return new ProverObjectContext(options);
        }

        public static ProverObjectContext CreateWithSqlServer(string connString)
        {
            var connection = new SqlConnection(connString);

            var options = new DbContextOptionsBuilder<ProverObjectContext>()
                .UseSqlServer(connection)
                .Options;

            return new ProverObjectContext(options);
        }
    }


    public class ProverObjectContext : DbContext, IDbContext
    {
        public ProverObjectContext(DbContextOptions options) : base(options)
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
                optionsBuilder.UseSqlServer(@"Server=.\sqlexpress;Database=Prover_New;Trusted_Connection=True;");

            optionsBuilder.Configure(
                From.AssemblyOf<VerificationRun>(new ProverEntityAutoConfiguration())
                    .UseOverridesFromAssemblyOf<ProverObjectContext>());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    // Pick up all concrete types that inherit from Entity
    public class ProverEntityAutoConfiguration : DefaultEntityAutoConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            var isMap = IsSubclassOfRawGeneric(typeof(Entity), type);
            return base.ShouldMap(type) && isMap;
        }

        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                    return true;
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}