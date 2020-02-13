using System.Text.Json;
using System.Threading.Tasks;
using Application.Services;
using Application.ViewModels.Services;
using Domain.EvcVerifications;
using Infrastructure.EntityFrameworkSqlDataAccess;
using Infrastructure.EntityFrameworkSqlDataAccess.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Interfaces;

namespace Infrastructure.Console
{
    public class Program
    {

        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("System.Text.Json", LogLevel.Trace)
                    .AddFilter("Infrastructure", LogLevel.Debug)
                    .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Debug)
                    .AddFilter("JsonSerializer", LogLevel.Trace)
                    .AddConsole();
            });

        private static readonly string connectionString = "Server=.\\sqlexpress;Database=prover;Trusted_Connection=True;";

        #region Public Methods

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<ProverDbContext>(c =>
                        c.UseLoggerFactory(MyLoggerFactory)
                            .EnableSensitiveDataLogging() 
                            .UseSqlServer(connectionString));

                    services.AddScoped<IAsyncRepository<EvcVerificationTest>>(c => new EfRepository<EvcVerificationTest>(c.GetService<ProverDbContext>()));

                    services.AddScoped<EvcVerificationTestService>();

                    services.AddScoped<DbInitializer>();

                    services.AddScoped<VerificationViewModelService>();
                });
        }

        public static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
            var context = host.Services.GetRequiredService<ProverDbContext>();

            var db = host.Services.GetRequiredService<DbInitializer>();
            await db.Initialize(context);

            host.Start();
        }

        #endregion
    }
}