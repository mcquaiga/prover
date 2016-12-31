namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClientItems : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClientItems",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ClientId = c.Guid(nullable: false),
                        ItemFileType = c.String(maxLength: 4000),
                        InstrumentData = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .Index(t => t.ClientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClientItems", "ClientId", "dbo.Clients");
            DropIndex("dbo.ClientItems", new[] { "ClientId" });
            DropTable("dbo.ClientItems");
        }
    }
}
