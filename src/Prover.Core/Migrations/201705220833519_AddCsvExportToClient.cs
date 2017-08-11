using System.Data.Entity.Migrations;

namespace Prover.Core.Migrations
{
    public partial class AddCsvExportToClient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "CreateCertificateCsvFile", c => c.Boolean(false));
        }

        public override void Down()
        {
            DropColumn("dbo.Clients", "CreateCertificateCsvFile");
        }
    }
}