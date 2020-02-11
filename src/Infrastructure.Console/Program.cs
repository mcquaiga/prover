using System.Threading.Tasks;
using Infrastructure.EntityFrameworkSqlDataAccess.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Console
{
    public class Program
    {
        private static readonly string connectionString = "Server=.\\sqlexpress;Database=prover;Trusted_Connection=True;";

        #region Public Methods

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<ProverDbContext>(c =>
                        c.UseSqlServer(connectionString));
                });
        }

        public static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
            var context = host.Services.GetRequiredService<ProverDbContext>();
            await context.Database.EnsureCreatedAsync();
            
            host.Start();
        }

        #endregion
    }
}