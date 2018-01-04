using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlServerCe;
using System.Diagnostics;
using NLog;
using Prover.Core.Migrations;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Z.EntityFramework.Plus;
using Prover.Core.Shared.Domain;

//using Prover.Core.Migrations;

namespace Prover.Core.Storage
{
    public class ProverContext : DbContext
    {
        private Logger _log = LogManager.GetCurrentClassLogger();

        public ProverContext()
            : base(@"name=ConnectionString")
        {
            _log.Trace("Starting Db Context...");

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ProverContext, Configuration>());
            ((IObjectContextAdapter) this).ObjectContext.ObjectMaterialized += ObjectContext_ObjectMaterialized;
            
            Database.Log = s => Debug.WriteLine(s);
        }

        public DbSet<VerificationTest> VerificationTests { get; set; }
        public DbSet<VolumeTest> VolumeTests { get; set; }
        public DbSet<TemperatureTest> TemperatureTests { get; set; }
        public DbSet<PressureTest> PressureTests { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Instrument> Instruments { get; set; }

        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientItems> ClientItemses { get; set; }
        public DbSet<ClientCsvTemplate> ClientCsvTemplates { get; set; }
        public DbSet<KeyValue> KeyValueStore { get; set; }

        protected void ObjectContext_ObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            (e.Entity as EntityWithId)?.OnInitializing();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (Database.Connection is SqlCeConnection)
                QueryIncludeOptimizedManager.AllowQueryBatch = false;

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            base.OnModelCreating(modelBuilder);            
        }
    }
}