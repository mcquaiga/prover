using System.Data.Entity;
using System.Diagnostics;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;


namespace Prover.Modules.Certificates.Storage
{
    //public class CertificateContext : DbContext
    //{
    //    public CertificateContext() : base(@"name=CertificateConnectionString")
    //    {
    //        //Database.SetInitializer(new MigrateDatabaseToLatestVersion<CertificateContext, Configuration>());
    //        Database.Initialize(false);
    //        Database.Log = s => Debug.WriteLine(s);
    //    }

    //    public DbSet<Certificate> Certificates { get; set; }

    //    public DbSet<Instrument> Instruments { get; set; }
    //    public DbSet<VerificationTest> VerificationTests { get; set; }
    //    public DbSet<VolumeTest> VolumeTests { get; set; }
    //    public DbSet<TemperatureTest> TemperatureTests { get; set; }
    //    public DbSet<PressureTest> PressureTests { get; set; }
    //}
}
