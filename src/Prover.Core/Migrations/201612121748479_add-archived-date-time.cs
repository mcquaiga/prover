namespace Prover.Core.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Addarchiveddatetime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Instruments", "ArchivedDateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Instruments", "ArchivedDateTime");
        }
    }
}
