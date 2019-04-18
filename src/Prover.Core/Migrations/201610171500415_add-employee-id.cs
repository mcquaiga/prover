using System.Data.Entity.Migrations;

namespace Prover.Core.Migrations
{
    public partial class addemployeeid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Instruments", "EmployeeId", c => c.String(maxLength: 4000));
        }

        public override void Down()
        {
            DropColumn("dbo.Instruments", "EmployeeId");
        }
    }
}