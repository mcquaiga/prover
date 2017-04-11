using Microsoft.EntityFrameworkCore;
using Prover.Data.EF.Models;
using Prover.Data.EF.Models.PressureTest;
using Prover.Data.EF.Models.TestPoint;
using Prover.Data.EF.Models.TestRun;

namespace Prover.Data.EF.Common
{
    internal class ProverDbContext : DbContext
    {
        public ProverDbContext(DbContextOptions<ProverDbContext> options)
        : base(options)
        { }

        public DbSet<TestRunDao> TestRuns { get; set; }
        public DbSet<TestPointDao> TestPoints { get; set; }
        public DbSet<PressureTestDao> PressureTests { get; set; }
        public DbSet<TemperatureTestDao> TemperatureTests { get; set; }
        public DbSet<VolumeTestDao> VolumeTests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
            }
            //optionsBuilder.UseSqlServer(@"Server=.\sqlexpress;Database=prover_api_vNext;Trusted_Connection=True;");
        }
    }
    

    
}
