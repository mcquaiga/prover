using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Prover.Core.Storage;
using Prover.Modules.Certificates.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Prover.Web.API.Storage
{
    public class DataContext : DbContext
    {
        public DataContext()
            : base()
        {
            this.LogToConsole();
        }

        public DbSet<Certificate> Certificates { get; set; }

        public DbSet<VerificationTest> VerificationTests { get; set; }
        public DbSet<VolumeTest> VolumeTests { get; set; }
        public DbSet<TemperatureTest> TemperatureTests { get; set; }
        public DbSet<PressureTest> PressureTests { get; set; }
        public DbSet<Instrument> Instruments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"Server=.\sqlexpress;Database=prover_api;Trusted_Connection=True;");
        }
    }

    internal static class DbContextExtensions
    {
        public static void LogToConsole(this DbContext context)
        {
            var loggerFactory = context.GetService<ILoggerFactory>();
            loggerFactory.AddConsole(LogLevel.Debug);
        }
    }
}
