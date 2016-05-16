namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Certificates",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false),
                        VerificationType = c.String(maxLength: 4000),
                        TestedBy = c.String(maxLength: 4000),
                        Number = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Instruments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TestDateTime = c.DateTime(nullable: false),
                        Type = c.Int(nullable: false),
                        CertificateId = c.Guid(),
                        InstrumentData = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Certificates", t => t.CertificateId)
                .Index(t => t.CertificateId);
            
            CreateTable(
                "dbo.Temperatures",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        InstrumentId = c.Guid(nullable: false),
                        InstrumentData = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Instruments", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.TemperatureTests",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TemperatureId = c.Guid(nullable: false),
                        IsVolumeTestTemperature = c.Boolean(nullable: false),
                        TestLevel = c.Int(nullable: false),
                        Gauge = c.Double(nullable: false),
                        InstrumentData = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Temperatures", t => t.TemperatureId)
                .Index(t => t.TemperatureId);
            
            CreateTable(
                "dbo.Volumes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PulseACount = c.Int(nullable: false),
                        PulseBCount = c.Int(nullable: false),
                        AppliedInput = c.Double(nullable: false),
                        InstrumentId = c.Guid(nullable: false),
                        TestInstrumentData = c.String(maxLength: 4000),
                        InstrumentData = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Instruments", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Volumes", "Id", "dbo.Instruments");
            DropForeignKey("dbo.TemperatureTests", "TemperatureId", "dbo.Temperatures");
            DropForeignKey("dbo.Temperatures", "Id", "dbo.Instruments");
            DropForeignKey("dbo.Instruments", "CertificateId", "dbo.Certificates");
            DropIndex("dbo.Volumes", new[] { "Id" });
            DropIndex("dbo.TemperatureTests", new[] { "TemperatureId" });
            DropIndex("dbo.Temperatures", new[] { "Id" });
            DropIndex("dbo.Instruments", new[] { "CertificateId" });
            DropTable("dbo.Volumes");
            DropTable("dbo.TemperatureTests");
            DropTable("dbo.Temperatures");
            DropTable("dbo.Instruments");
            DropTable("dbo.Certificates");
        }
    }
}
