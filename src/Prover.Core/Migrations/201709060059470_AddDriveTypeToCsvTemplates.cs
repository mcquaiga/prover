namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDriveTypeToCsvTemplates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientCsvTemplates", "DriveType", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClientCsvTemplates", "DriveType");
        }
    }
}
