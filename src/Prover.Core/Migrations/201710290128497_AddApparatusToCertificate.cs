namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddApparatusToCertificate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Certificates", "Apparatus", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Certificates", "Apparatus");
        }
    }
}
