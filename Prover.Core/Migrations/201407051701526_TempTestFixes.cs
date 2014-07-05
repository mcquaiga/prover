namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TempTestFixes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TemperatureTests", "InstrumentData", c => c.String());
            DropColumn("dbo.TemperatureTests", "Data");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TemperatureTests", "Data", c => c.String());
            DropColumn("dbo.TemperatureTests", "InstrumentData");
        }
    }
}
