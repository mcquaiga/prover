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

        internal DbSet<EvcVerificationDto> EvcVerifications { get; set; }

        internal DbSet<VerificationTestPointJson> VerificationTestPoints { get; set; }

        internal DbSet<VerificationTestFactorJson> VerificationTests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}