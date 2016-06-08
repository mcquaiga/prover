namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class instrumenttype : DbMigration
    {
        public override void Up()
        {
            //DropColumn("dbo.Instruments", "Type");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.Instruments", "Type", c => c.Int(nullable: false));
        }
    }
}
