using System.Data.Entity.Migrations;

namespace Prover.Core.Migrations
{
    public partial class refactorpt : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.PressureTests", "TestLevel");
            DropColumn("dbo.TemperatureTests", "TestLevel");
        }

        public override void Down()
        {
            AddColumn("dbo.TemperatureTests", "TestLevel", c => c.Int(false));
            AddColumn("dbo.PressureTests", "TestLevel", c => c.Int(false));
        }
    }
}