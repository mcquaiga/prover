namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddKeyValueTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KeyValues",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 4000),
                        Value = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.KeyValues");
        }
    }
}
