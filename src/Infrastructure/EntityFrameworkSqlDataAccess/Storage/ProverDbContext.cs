using System.Reflection;
using Infrastructure.EntityFrameworkSqlDataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFrameworkSqlDataAccess.Storage
{
    public class ProverDbContext : DbContext
    {
        public ProverDbContext(){}

        public ProverDbContext(DbContextOptions<ProverDbContext> options) : base(options)
        {
        }

        internal DbSet<EvcVerificationSql> EvcVerifications { get; set; }

        //internal DbSet<VerificationTestPoint> TestPoints { get; set; }

        //internal DbSet<VerificationTestEntity> VerificationTests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}