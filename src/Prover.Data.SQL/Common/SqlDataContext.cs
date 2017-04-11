using Microsoft.EntityFrameworkCore;

namespace Prover.Data.EF.Common
{
    public class SqlDataContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"Server=.\sqlexpress;Database=prover_api_vNext;Trusted_Connection=True;");
        }        
    }
}
