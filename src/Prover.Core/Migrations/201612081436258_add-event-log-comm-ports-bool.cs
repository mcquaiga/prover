using System.Data.Entity.Migrations;

namespace Prover.Core.Migrations
{
    public partial class addeventlogcommportsbool : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Instruments", "EventLogPassed", c => c.Boolean());
            AddColumn("dbo.Instruments", "CommPortsPassed", c => c.Boolean());
        }

        public override void Down()
        {
            DropColumn("dbo.Instruments", "CommPortsPassed");
            DropColumn("dbo.Instruments", "EventLogPassed");
        }
    }
}