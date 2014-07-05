using Prover.Core.Models.Instruments;
using Prover.Core.Storage;

namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Prover.Core.Storage.ProverContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ProverContext context)
        {
              //This method will be called after migrating to the latest version.

              //You can use the DbSet<T>.AddOrUpdate() helper extension method 
              //to avoid creating duplicate seed data. E.g.
            
              //  context.People.AddOrUpdate(
              //    p => p.FullName,
              //    new Person { FullName = "Andrew Peters" },
              //    new Person { FullName = "Brice Lambson" },
              //    new Person { FullName = "Rowan Miller" }
              //  );
            var inst = new Instrument {InstrumentData = @"[
                {'Number': 0,'Value': 1093.0},
                {'Number': 2,'Value': 157.0},
                {'Number': 5,'Value': 0.0}]",
                CertificateGuid = Guid.Empty,
                SerialNumber = 123456,
                Type = InstrumentType.MiniMax
            };

            context.Instruments.Add(inst);


        }
    }
}
