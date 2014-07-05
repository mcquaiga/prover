namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Items : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Instruments", "InstrumentData", c => c.String());
            DropColumn("dbo.Instruments", "Data");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Instruments", "Data", c => c.String());
            DropColumn("dbo.Instruments", "InstrumentData");
        }
    }
}
