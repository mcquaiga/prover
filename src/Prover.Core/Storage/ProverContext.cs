using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using Prover.Core.Migrations;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;

//using Prover.Core.Migrations;

namespace Prover.Core.Storage
{
    public class ProverContext : DbContext
    {
        public ProverContext()
            : base(@"name=ConnectionString")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ProverContext, Configuration>());
            Database.Initialize(false);
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

        protected void ObjectContext_ObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            (e.Entity as BaseEntity)?.OnInitializing();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}