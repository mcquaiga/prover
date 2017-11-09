namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClientRegisterationNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "RegistrationNumber", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "RegistrationNumber");
        }
    }
}
