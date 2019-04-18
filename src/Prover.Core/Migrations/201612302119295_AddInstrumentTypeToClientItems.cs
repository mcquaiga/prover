using System.Data.Entity.Migrations;

namespace Prover.Core.Migrations
{
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