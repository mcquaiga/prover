namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class exportdatetime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Instruments", "ExportedDateTime", c => c.DateTime(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Instruments", "ExportedDateTime");
        }
    }
}
