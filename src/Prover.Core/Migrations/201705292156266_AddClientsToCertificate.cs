using System.Data.Entity.Migrations;

namespace Prover.Core.Migrations
{
    public partial class AddClientsToCertificate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Certificates", "ClientId", c => c.Guid());
            CreateIndex("dbo.Certificates", "ClientId");
            AddForeignKey("dbo.Certificates", "ClientId", "dbo.Clients", "Id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Certificates", "ClientId", "dbo.Clients");
            DropIndex("dbo.Certificates", new[] {"ClientId"});
            DropColumn("dbo.Certificates", "ClientId");
        }
    }
}