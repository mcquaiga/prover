using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Storage
{
    public class ProverDbContext : DbContext
    {
        public ProverDbContext(DbContextOptions<ProverDbContext> options) : base(options)
        {
        }
    }
}