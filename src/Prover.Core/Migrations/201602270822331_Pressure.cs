namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Pressure : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pressures",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        InstrumentId = c.Guid(nullable: false),
                        InstrumentData = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Instruments", t => t.InstrumentId)
                .Index(t => t.InstrumentId);
            
            CreateTable(
                "dbo.PressureTests",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TestLevel = c.Int(nullable: false),
                        InstrumentData = c.String(maxLength: 4000),
                        Pressure_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pressures", t => t.Pressure_Id)
                .Index(t => t.Pressure_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PressureTests", "Pressure_Id", "dbo.Pressures");
            DropForeignKey("dbo.Pressures", "InstrumentId", "dbo.Instruments");
            DropIndex("dbo.PressureTests", new[] { "Pressure_Id" });
            DropIndex("dbo.Pressures", new[] { "InstrumentId" });
            DropTable("dbo.PressureTests");
            DropTable("dbo.Pressures");
        }
    }
}
