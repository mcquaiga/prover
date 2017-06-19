using System.Data.Entity.Migrations;

namespace Prover.Core.Migrations
{
    public partial class AddClientCsvTemplates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.ClientCsvTemplates",
                    c => new
                    {
                        Id = c.Guid(false),
                        ClientId = c.Guid(false),
                        VerificationType = c.String(maxLength: 4000),
                        InstrumentType = c.String(maxLength: 4000),
                        CorrectorType = c.String(maxLength: 4000),
                        CsvTemplate = c.String(maxLength: 4000)
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .Index(t => t.ClientId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.ClientCsvTemplates", "ClientId", "dbo.Clients");
            DropIndex("dbo.ClientCsvTemplates", new[] {"ClientId"});
            DropTable("dbo.ClientCsvTemplates");
        }
    }
}