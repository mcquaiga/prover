namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PressureTables : DbMigration
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
                        PressureId = c.Guid(nullable: false),
                        TestLevel = c.Int(nullable: false),
                        IsVolumeTestPressure = c.Boolean(nullable: false),
                        GasGauge = c.Decimal(precision: 18, scale: 2),
                        AtmosphericGauge = c.Decimal(precision: 18, scale: 2),
                        InstrumentData = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pressures", t => t.PressureId)
                .Index(t => t.PressureId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PressureTests", "PressureId", "dbo.Pressures");
            DropForeignKey("dbo.Pressures", "InstrumentId", "dbo.Instruments");
            DropIndex("dbo.PressureTests", new[] { "PressureId" });
            DropIndex("dbo.Pressures", new[] { "InstrumentId" });
            DropTable("dbo.PressureTests");
            DropTable("dbo.Pressures");
        }
    }
}
