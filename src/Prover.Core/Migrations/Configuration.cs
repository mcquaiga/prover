using System.Data.Entity.Migrations;
using Prover.Core.Storage;

namespace Prover.Core.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ProverContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Prover.Core.Storage.ProverContext";
        }

        protected override void Seed(ProverContext context)
        {
        }
    }
}