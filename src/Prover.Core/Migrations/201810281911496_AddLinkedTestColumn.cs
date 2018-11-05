namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLinkedTestColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Instruments", "LinkedTestId", c => c.Guid());
            CreateIndex("dbo.Instruments", "LinkedTestId");
            AddForeignKey("dbo.Instruments", "LinkedTestId", "dbo.Instruments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Instruments", "LinkedTestId", "dbo.Instruments");
            DropIndex("dbo.Instruments", new[] { "LinkedTestId" });
            DropColumn("dbo.Instruments", "LinkedTestId");
        }
    }
}
