namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMasterSlavegroupings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Instruments", "ParentTestId", c => c.Guid());
            CreateIndex("dbo.Instruments", "ParentTestId");
            AddForeignKey("dbo.Instruments", "ParentTestId", "dbo.Instruments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Instruments", "ParentTestId", "dbo.Instruments");
            DropIndex("dbo.Instruments", new[] { "ParentTestId" });
            DropColumn("dbo.Instruments", "ParentTestId");
        }
    }
}
