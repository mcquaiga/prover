using System.Linq;
using System.Reflection;
using Domain.EvcVerifications.CorrectionTests;
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

        internal DbSet<VerificationTestPointSql> VerificationTestPoints { get; set; }

        internal DbSet<BaseVerificationTestSql> VerificationTests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<EvcVerificationSql>()
                .HasMany<BaseVerificationTestSql>(evc => evc.ChildTests);

            builder.Entity<EvcVerificationSql>().OwnsMany(evc => evc.ChildTests)
                .WithOwner();

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlite(connectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}