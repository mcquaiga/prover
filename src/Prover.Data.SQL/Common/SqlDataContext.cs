using Microsoft.EntityFrameworkCore;
using Prover.Data.EF.Models;
using Prover.Data.EF.Models.PressureTest;
using Prover.Data.EF.Models.TestPoint;
using Prover.Data.EF.Models.TestRun;

namespace Prover.Data.EF.Common
{
    public class SqlDataContext : DbContext
    {
        internal DbSet<TestRunDatabase> TestRuns { get; set; }
        internal DbSet<TestPointDatabase> TestPoints { get; set; }
        internal DbSet<PressureTestDatabase> PressureTests { get; set; }
        internal DbSet<TemperatureTestDatabase> TemperatureTests { get; set; }
        internal DbSet<VolumeTestDatabase> VolumeTests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"Server=.\sqlexpress;Database=Prover_New;Trusted_Connection=True;");
        }        
    }
}
