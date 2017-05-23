namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCsvExportToClient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "CreateCertificateCsvFile", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "CreateCertificateCsvFile");
        }
    }
}
