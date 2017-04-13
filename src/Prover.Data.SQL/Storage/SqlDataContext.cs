using Microsoft.EntityFrameworkCore;
using Prover.Data.EF.Models.Prover;
using Prover.Data.EF.Models.TestRun;

namespace Prover.Data.EF.Storage
{
    public class SqlDataContext : DbContext
    {
        internal DbSet<TestRunDatabase> TestRuns { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"Server=.\sqlexpress;Database=Prover_New;Trusted_Connection=True;");
        }        
    }
}
