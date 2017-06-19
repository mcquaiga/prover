using System.Data.Entity.Migrations;

namespace Prover.Core.Migrations
{
    public partial class addjobid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Instruments", "JobId", c => c.String(maxLength: 4000));
        }

        public override void Down()
        {
            DropColumn("dbo.Instruments", "JobId");
        }
    }
}