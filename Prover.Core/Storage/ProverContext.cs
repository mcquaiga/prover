using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Prover.Core.Migrations;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;

namespace Prover.Core.Storage
{
    public class ProverContext : DbContext, IProverContext
    {

        public ProverContext()
            : base(@"name=ConnectionString")
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<ProverContext, Configuration>());
            Database.Initialize(false);
        }

        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Instrument> Instruments { get; set; }
        public DbSet<Volume> Volume { get; set; }
        public DbSet<Temperature> Temperatures { get; set; }
        public DbSet<TemperatureTest> TemperatureTests { get; set; }
    }
}
