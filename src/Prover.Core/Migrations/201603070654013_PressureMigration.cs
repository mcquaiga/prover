namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PressureMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pressures",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        InstrumentId = c.Guid(nullable: false),
                        InstrumentData = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Instruments", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.PressureTests",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PressureId = c.Guid(nullable: false),
                        TestLevel = c.Int(nullable: false),
                        IsVolumeTestPressure = c.Boolean(nullable: false),
                        GasGauge = c.Decimal(precision: 18, scale: 2),
                        AtmosphericGauge = c.Decimal(precision: 18, scale: 2),
                        InstrumentData = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pressures", t => t.PressureId)
                .Index(t => t.PressureId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PressureTests", "PressureId", "dbo.Pressures");
            DropForeignKey("dbo.Pressures", "Id", "dbo.Instruments");
            DropIndex("dbo.PressureTests", new[] { "PressureId" });
            DropIndex("dbo.Pressures", new[] { "Id" });
            DropTable("dbo.PressureTests");
            DropTable("dbo.Pressures");
        }
    }
}
