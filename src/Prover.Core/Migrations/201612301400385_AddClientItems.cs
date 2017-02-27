using System.Data.Entity.Migrations;

namespace Prover.Core.Migrations
{
    public partial class AddClientItems : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.ClientItems",
                    c => new
                    {
                        Id = c.Guid(false),
                        ClientId = c.Guid(false),
                        ItemFileType = c.String(maxLength: 4000),
                        InstrumentData = c.String(maxLength: 4000)
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .Index(t => t.ClientId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.ClientItems", "ClientId", "dbo.Clients");
            DropIndex("dbo.ClientItems", new[] {"ClientId"});
            DropTable("dbo.ClientItems");
        }
    }
}