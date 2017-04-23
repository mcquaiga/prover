namespace Prover.Core.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddClientIdToInstrument : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Instruments", "ClientId", c => c.Guid());
            CreateIndex("dbo.Instruments", "ClientId");
            AddForeignKey("dbo.Instruments", "ClientId", "dbo.Clients", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Instruments", "ClientId", "dbo.Clients");
            DropIndex("dbo.Instruments", new[] { "ClientId" });
            DropColumn("dbo.Instruments", "ClientId");
        }
    }
}
