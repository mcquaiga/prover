using System.Text.Json;
using System.Threading.Tasks;
using Application.Services;
using Application.ViewModels.Services;
using Domain.EvcVerifications;
using Infrastructure.EntityFrameworkSqlDataAccess;
using Infrastructure.EntityFrameworkSqlDataAccess.Storage;
using Infrastructure.KeyValueStore;
using LiteDB;
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
        private static string litDbConnectionString = "cached_data.db";

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
                    //services.AddScoped<IAsyncRepository<EvcVerificationTest>>(c => new EfRepository<EvcVerificationTest>(c.GetService<ProverDbContext>()));
                    //services.AddScoped<DbInitializer>();

                    var deviceRepo = Devices.RepositoryFactory.CreateDefault();
                    services.AddSingleton<ILiteDatabase>(c => new LiteDatabase(litDbConnectionString));
                    services.AddScoped<IAsyncRepository<EvcVerificationTest>>(c => new VerificationsLiteDbRepository(c.GetService<ILiteDatabase>(), deviceRepo));
                    //services.AddSingleton(c => ActivatorUtilities.CreateInstance<LiteDbInitializer>(c, deviceRepo));


                    services.AddScoped<EvcVerificationTestService>();
                    services.AddScoped<VerificationViewModelService>();
                });
        }

        public static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
            var context = host.Services.GetRequiredService<ProverDbContext>();

            //var db = host.Services.GetRequiredService<DbInitializer>();
            //await db.Initialize(context);
            //var db = host.Services.GetRequiredService<LiteDbInitializer>();
            //db.Initialize();

            host.Start();
        }

        #endregion
    }
}