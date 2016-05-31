namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateexporteddatetime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Instruments", "ExportedDateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Instruments", "ExportedDateTime", c => c.DateTime(nullable: false));
        }
    }
}
