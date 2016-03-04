namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Pressures", "InstrumentId", "dbo.Instruments");
            DropForeignKey("dbo.PressureTests", "Pressure_Id", "dbo.Pressures");
            DropIndex("dbo.Pressures", new[] { "InstrumentId" });
            DropIndex("dbo.PressureTests", new[] { "Pressure_Id" });
            DropTable("dbo.Pressures");
            DropTable("dbo.PressureTests");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PressureTests",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TestLevel = c.Int(nullable: false),
                        InstrumentData = c.String(maxLength: 4000),
                        Pressure_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Pressures",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        InstrumentId = c.Guid(nullable: false),
                        InstrumentData = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.PressureTests", "Pressure_Id");
            CreateIndex("dbo.Pressures", "InstrumentId");
            AddForeignKey("dbo.PressureTests", "Pressure_Id", "dbo.Pressures", "Id");
            AddForeignKey("dbo.Pressures", "InstrumentId", "dbo.Instruments", "Id");
        }
    }
}
