using Microsoft.EntityFrameworkCore;
using Module.EvcVerification.Models;

namespace Infrastructure.Storage
{
    public class ProverDbContext : DbContext
    {
        #region Constructors

        public ProverDbContext(DbContextOptions<ProverDbContext> options) : base(options)
        {
        }

        public DbSet<EvcVerification> Verifications { get; set; }

        public DbSet<Clients>

        #endregion
    }
}