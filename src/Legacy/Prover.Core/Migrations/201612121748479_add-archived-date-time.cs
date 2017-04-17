using System.Data.Entity.Migrations;

namespace Prover.Core.Migrations
{
    public partial class Addarchiveddatetime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Instruments", "ArchivedDateTime", c => c.DateTime());
        }

        public override void Down()
        {
            DropColumn("dbo.Instruments", "ArchivedDateTime");
        }
    }
}