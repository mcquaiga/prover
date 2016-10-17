namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addemployeeid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Instruments", "EmployeeId", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Instruments", "EmployeeId");
        }
    }
}
