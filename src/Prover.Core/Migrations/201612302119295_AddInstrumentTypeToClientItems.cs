namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInstrumentTypeToClientItems : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientItems", "InstrumentType", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClientItems", "InstrumentType");
        }
    }
}
