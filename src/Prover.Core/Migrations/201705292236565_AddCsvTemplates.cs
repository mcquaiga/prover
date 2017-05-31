namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCsvTemplates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClientCsvTemplatescs",
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
            DropForeignKey("dbo.ClientCsvTemplatescs", "ClientId", "dbo.Clients");
            DropIndex("dbo.ClientCsvTemplatescs", new[] { "ClientId" });
            DropTable("dbo.ClientCsvTemplatescs");
        }
    }
}
