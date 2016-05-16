namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class refactorpt : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.PressureTests", "TestLevel");
            DropColumn("dbo.TemperatureTests", "TestLevel");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TemperatureTests", "TestLevel", c => c.Int(nullable: false));
            AddColumn("dbo.PressureTests", "TestLevel", c => c.Int(nullable: false));
        }
    }
}
