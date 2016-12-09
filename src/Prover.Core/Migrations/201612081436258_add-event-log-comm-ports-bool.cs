namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
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
