using Microsoft.EntityFrameworkCore;
using Prover.Core.DAL.DataAccess.QaTestRuns;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Prover.Core.DAL
{
    internal class DataContext : DbContext
    {
        internal DbSet<QaTestRunDal> QaTestRuns { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"Server=.\sqlexpress;Database=prover_api_vNext;Trusted_Connection=True;");
        }        
    }
}
