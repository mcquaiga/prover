using System.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Prover.Storage.EF.Storage
{
    public class SqliteContextFactory : IDbContextFactory<EfProverContext>
    {
        public EfProverContext Create(DbContextFactoryOptions options)
        {
            var connection = new SqliteConnection($"Data Source = {options.ApplicationBasePath}\\prover_data.db");
            connection.Open();

            var optionsBuilder = new DbContextOptionsBuilder<EfProverContext>()
                .UseSqlite(connection)
                .Options;

            return new EfProverContext(optionsBuilder);
        }
    }

    public static class DataContextFactory
    {
        public static EfProverContext CreateWithSqlite(string connString)
        {
            var connection = new SqliteConnection(connString);

            var options = new DbContextOptionsBuilder<EfProverContext>()
                .UseSqlite(connection)
                .Options;

            return new EfProverContext(options);
        }

        public static EfProverContext CreateWithSqlServer(string connString)
        {
            var connection = new SqlConnection(connString);

            var options = new DbContextOptionsBuilder<EfProverContext>()
                .UseSqlServer(connection)
                .Options;

            return new EfProverContext(options);
        }
    }
}