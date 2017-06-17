namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClientCsvTemplates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClientCsvTemplates",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ClientId = c.Guid(nullable: false),
                        VerificationType = c.Int(nullable: false),
                        CorrectorType = c.Int(nullable: false),
                        CsvTemplate = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .Index(t => t.ClientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClientCsvTemplates", "ClientId", "dbo.Clients");
            DropIndex("dbo.ClientCsvTemplates", new[] { "ClientId" });
            DropTable("dbo.ClientCsvTemplates");
        }
    }
}
