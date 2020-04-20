using System.Data.Entity.Migrations;

namespace DataMigrator
{
    public sealed class Configuration : DbMigrationsConfiguration<MigrateContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "DataMigrator.MigrateContext";
        }    
    }
}