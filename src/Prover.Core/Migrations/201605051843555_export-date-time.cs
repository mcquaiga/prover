using System.Data.Entity.Migrations;

namespace Prover.Core.Migrations
{
    public partial class exportdatetime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Instruments", "ExportedDateTime", c => c.DateTime(true));
        }

        public override void Down()
        {
            DropColumn("dbo.Instruments", "ExportedDateTime");
        }
    }
}