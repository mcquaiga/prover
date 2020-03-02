using System.Threading.Tasks;
using Infrastructure.EntityFrameworkSqlDataAccess.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Console
{
    public class Startup
    {
        private static string connectionString = "Data Source=prover_data.db;";
        

        IConfigurationRoot Configuration { get; }

        public Startup()
        {
            var builder = new ConfigurationBuilder();//.Add().AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public async Task ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddSingleton<IConfigurationRoot>(Configuration);
            
            await ConfigureDbServices(services);
        }

        public async Task ConfigureDbServices(IServiceCollection services)
        {
            services.AddDbContext<ProverDbContext>(c => 
                c.UseSqlite(connectionString));

        }
    }
}
