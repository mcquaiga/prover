namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TempFixes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Temperatures", "InstrumentData", c => c.String());
            DropColumn("dbo.Temperatures", "Data");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Temperatures", "Data", c => c.String());
            DropColumn("dbo.Temperatures", "InstrumentData");
        }
    }
}
