namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class verifications : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Pressures", "Id", "dbo.Instruments");
            DropForeignKey("dbo.PressureTests", "PressureId", "dbo.Pressures");
            DropForeignKey("dbo.Temperatures", "Id", "dbo.Instruments");
            DropForeignKey("dbo.TemperatureTests", "TemperatureId", "dbo.Temperatures");
            DropForeignKey("dbo.Volumes", "Id", "dbo.Instruments");
            DropIndex("dbo.Pressures", new[] { "Id" });
            DropIndex("dbo.PressureTests", new[] { "PressureId" });
            DropIndex("dbo.Temperatures", new[] { "Id" });
            DropIndex("dbo.TemperatureTests", new[] { "TemperatureId" });
            DropIndex("dbo.Volumes", new[] { "Id" });
            CreateTable(
                "dbo.VerificationTests",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TestNumber = c.Int(nullable: false),
                        InstrumentId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Instruments", t => t.InstrumentId)
                .Index(t => t.InstrumentId);
            
            CreateTable(
                "dbo.VolumeTests",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PulseACount = c.Int(nullable: false),
                        PulseBCount = c.Int(nullable: false),
                        AppliedInput = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DriveTypeDiscriminator = c.String(),
                        TestInstrumentData = c.String(),
                        VerificationTestId = c.Guid(nullable: false),
                        InstrumentData = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VerificationTests", t => t.VerificationTestId)
                .Index(t => t.Id);
            
            AddColumn("dbo.PressureTests", "VerificationTestId", c => c.Guid(nullable: false));
            AddColumn("dbo.TemperatureTests", "VerificationTestId", c => c.Guid(nullable: false));
            CreateIndex("dbo.PressureTests", "Id");
            CreateIndex("dbo.TemperatureTests", "Id");
            AddForeignKey("dbo.PressureTests", "VerificationTestId", "dbo.VerificationTests", "Id");
            AddForeignKey("dbo.TemperatureTests", "VerificationTestId", "dbo.VerificationTests", "Id");
            DropColumn("dbo.PressureTests", "PressureId");
            DropColumn("dbo.PressureTests", "IsVolumeTestPressure");
            DropColumn("dbo.TemperatureTests", "TemperatureId");
            DropColumn("dbo.TemperatureTests", "IsVolumeTestTemperature");
            DropTable("dbo.Pressures");
            DropTable("dbo.Temperatures");
            DropTable("dbo.Volumes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Volumes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PulseACount = c.Int(nullable: false),
                        PulseBCount = c.Int(nullable: false),
                        AppliedInput = c.Double(nullable: false),
                        InstrumentId = c.Guid(nullable: false),
                        TestInstrumentData = c.String(),
                        InstrumentData = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Temperatures",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        InstrumentId = c.Guid(nullable: false),
                        InstrumentData = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Pressures",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        InstrumentId = c.Guid(nullable: false),
                        InstrumentData = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.TemperatureTests", "IsVolumeTestTemperature", c => c.Boolean(nullable: false));
            AddColumn("dbo.TemperatureTests", "TemperatureId", c => c.Guid(nullable: false));
            AddColumn("dbo.PressureTests", "IsVolumeTestPressure", c => c.Boolean(nullable: false));
            AddColumn("dbo.PressureTests", "PressureId", c => c.Guid(nullable: false));
            DropForeignKey("dbo.VolumeTests", "Id", "dbo.VerificationTests");
            DropForeignKey("dbo.TemperatureTests", "Id", "dbo.VerificationTests");
            DropForeignKey("dbo.PressureTests", "Id", "dbo.VerificationTests");
            DropForeignKey("dbo.VerificationTests", "InstrumentId", "dbo.Instruments");
            DropIndex("dbo.VolumeTests", new[] { "Id" });
            DropIndex("dbo.TemperatureTests", new[] { "Id" });
            DropIndex("dbo.PressureTests", new[] { "Id" });
            DropIndex("dbo.VerificationTests", new[] { "InstrumentId" });
            DropColumn("dbo.TemperatureTests", "VerificationTestId");
            DropColumn("dbo.PressureTests", "VerificationTestId");
            DropTable("dbo.VolumeTests");
            DropTable("dbo.VerificationTests");
            CreateIndex("dbo.Volumes", "Id");
            CreateIndex("dbo.TemperatureTests", "TemperatureId");
            CreateIndex("dbo.Temperatures", "Id");
            CreateIndex("dbo.PressureTests", "PressureId");
            CreateIndex("dbo.Pressures", "Id");
            AddForeignKey("dbo.Volumes", "Id", "dbo.Instruments", "Id");
            AddForeignKey("dbo.TemperatureTests", "TemperatureId", "dbo.Temperatures", "Id");
            AddForeignKey("dbo.Temperatures", "Id", "dbo.Instruments", "Id");
            AddForeignKey("dbo.PressureTests", "PressureId", "dbo.Pressures", "Id");
            AddForeignKey("dbo.Pressures", "Id", "dbo.Instruments", "Id");
        }
    }
}
