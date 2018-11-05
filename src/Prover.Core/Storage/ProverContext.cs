using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlServerCe;
using System.Diagnostics;
using Prover.Core.Migrations;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Z.EntityFramework.Plus;

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

        //public DbSet<GroupedInstruments> GroupedInstruments {get; set;}

        protected void ObjectContext_ObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            (e.Entity as BaseEntity)?.OnInitializing();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (Database.Connection is SqlCeConnection)
                QueryIncludeOptimizedManager.AllowQueryBatch = false;

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Instrument>()
                .HasMany(i => i.VerificationTests)
                .WithRequired(i => i.Instrument);

            modelBuilder.Entity<Instrument>()
                .HasOptional(i => i.LinkedTest);

            //modelBuilder.Entity<GroupedInstruments>()
            //    .HasKey(g => new { g.ParentId, g.ChildId });

            //modelBuilder.Entity<GroupedInstruments>()
            //    .HasRequired(p => p.Parent)
            //    .WithMany(c => c.ChildTests)
            //    .HasForeignKey(g => g.ParentId);

            //modelBuilder.Entity<GroupedInstruments>()
            //  .HasRequired(c => c.Child)
            //  .WithRequiredPrincipal(p => p.ParentTest);

            //modelBuilder.Entity<Instrument>()
            //    .HasMany(c => c.ChildTests)
            //    .WithRequired(i => i.Parent)                
            //    .HasForeignKey(f => f.ParentId);

            //modelBuilder.Entity<Instrument>()
            //    .HasOptional(p => p.ParentTest)
            //    .WithMany();           
            ;

            //.Map(m =>
            //{
            //    m.MapKey(new[] { "ChildId" });
            //    m.ToTable("GroupedInstruments");
            //});
            //.WithRequiredPrincipal(p => p.ParentTest);

            //modelBuilder.Entity<Instrument>()
            // .HasOptional(i => i.ParentTest)
            // .WithOptionalPrincipal()
            // .Map(m =>
            // {
            //     m.MapKey(new [] { "ParentId"});
            //     m.ToTable("GroupedInstruments");
            // });             

            //modelBuilder.Entity<Instrument>()
            //    .HasOptional(i => i.ChildTests)
            //    .WithOptionalDependent()
            //    .Map(m =>
            //    {
            //        m.MapKey("ChildId");                    
            //        m.ToTable("GroupedInstruments");
            //    });

            //modelBuilder.Entity<Instrument>()
            //    .HasMany(m => m.)
            //    .WithMany().Map(m =>
            //      {
            //          m.MapLeftKey("MemberId");
            //          m.MapRightKey("FriendId");
            //          m.ToTable("MembersFriends");
            //      }
            //  );              




            modelBuilder.Entity<VerificationTest>()
                .HasOptional(i => i.PressureTest);

            base.OnModelCreating(modelBuilder);
        }
    }
}