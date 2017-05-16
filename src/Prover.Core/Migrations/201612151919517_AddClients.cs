namespace Prover.Core.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddClients : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(maxLength: 4000),
                        Address = c.String(maxLength: 4000),
                        CreatedDateTime = c.DateTime(nullable: false),
                        ArchivedDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Clients");
        }
    }
}
