using System.Data.Entity.Migrations;

namespace Prover.Core.Migrations
{
    public partial class updateexporteddatetime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Instruments", "ExportedDateTime", c => c.DateTime());
        }

        public override void Down()
        {
            AlterColumn("dbo.Instruments", "ExportedDateTime", c => c.DateTime(false));
        }
    }
}