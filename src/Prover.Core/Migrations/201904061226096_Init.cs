namespace Prover.Core.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Certificates",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false),
                        VerificationType = c.String(),
                        TestedBy = c.String(),
                        Apparatus = c.String(),
                        ClientId = c.Guid(),
                        Number = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId)               
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(maxLength: 4000),
                        Address = c.String(),
                        RegistrationNumber = c.String(),
                        CreatedDateTime = c.DateTime(nullable: false),
                        ArchivedDateTime = c.DateTime(),
                        CreateCertificateCsvFile = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClientCsvTemplates",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ClientId = c.Guid(nullable: false),
                        VerificationType = c.String(),
                        InstrumentType = c.String(),
                        CorrectorType = c.String(),
                        DriveType = c.String(),
                        CsvTemplate = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.ClientItems",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ClientId = c.Guid(nullable: false),
                        ItemFileType = c.String(),
                        InstrumentType = c.String(),
                        InstrumentData = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.Instruments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ArchivedDateTime = c.DateTime(),
                        CertificateId = c.Guid(),
                        ClientId = c.Guid(),
                        CommPortsPassed = c.Boolean(),
                        EmployeeId = c.String(),
                        EventLogPassed = c.Boolean(),
                        ExportedDateTime = c.DateTime(),
                        JobId = c.String(),
                        TestDateTime = c.DateTime(nullable: false),
                        Type = c.Int(nullable: false),
                        LinkedTestId = c.Guid(),
                        InstrumentData = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Certificates", t => t.CertificateId)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .ForeignKey("dbo.Instruments", t => t.LinkedTestId)
                .Index(t => t.ArchivedDateTime)
                .Index(t => t.CertificateId)
                .Index(t => t.ClientId)
                .Index(t => t.ExportedDateTime)
                .Index(t => t.LinkedTestId);
            
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
                "dbo.FrequencyTests",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        VerificationTestId = c.Guid(nullable: false),
                        MainRotorPulseCount = c.Long(nullable: false),
                        SenseRotorPulseCount = c.Long(nullable: false),
                        MechanicalOutputFactor = c.Long(nullable: false),
                        ItemsType = c.String(),
                        PreTestItemData = c.String(),
                        PostTestItemData = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VerificationTests", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.PressureTests",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AtmosphericGauge = c.Decimal(precision: 18, scale: 2),
                        GasGauge = c.Decimal(precision: 18, scale: 2),
                        VerificationTestId = c.Guid(nullable: false),
                        InstrumentData = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VerificationTests", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.TemperatureTests",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Gauge = c.Double(nullable: false),
                        VerificationTestId = c.Guid(nullable: false),
                        InstrumentData = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VerificationTests", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.VolumeTests",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AppliedInput = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DriveTypeDiscriminator = c.String(),
                        PulseACount = c.Int(nullable: false),
                        PulseBCount = c.Int(nullable: false),
                        TestInstrumentData = c.String(),
                        VerificationTestId = c.Guid(nullable: false),
                        InstrumentData = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VerificationTests", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.KeyValues",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VolumeTests", "Id", "dbo.VerificationTests");
            DropForeignKey("dbo.TemperatureTests", "Id", "dbo.VerificationTests");
            DropForeignKey("dbo.PressureTests", "Id", "dbo.VerificationTests");
            DropForeignKey("dbo.VerificationTests", "InstrumentId", "dbo.Instruments");
            DropForeignKey("dbo.FrequencyTests", "Id", "dbo.VerificationTests");
            DropForeignKey("dbo.Instruments", "LinkedTestId", "dbo.Instruments");
            DropForeignKey("dbo.Instruments", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.Instruments", "CertificateId", "dbo.Certificates");
            DropForeignKey("dbo.Certificates", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.ClientItems", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.ClientCsvTemplates", "ClientId", "dbo.Clients");
            DropIndex("dbo.VolumeTests", new[] { "Id" });
            DropIndex("dbo.TemperatureTests", new[] { "Id" });
            DropIndex("dbo.PressureTests", new[] { "Id" });
            DropIndex("dbo.FrequencyTests", new[] { "Id" });
            DropIndex("dbo.VerificationTests", new[] { "InstrumentId" });
            DropIndex("dbo.Instruments", new[] { "LinkedTestId" });
            DropIndex("dbo.Instruments", new[] { "ExportedDateTime" });
            DropIndex("dbo.Instruments", new[] { "ClientId" });
            DropIndex("dbo.Instruments", new[] { "CertificateId" });
            DropIndex("dbo.Instruments", new[] { "ArchivedDateTime" });
            DropIndex("dbo.ClientItems", new[] { "ClientId" });
            DropIndex("dbo.ClientCsvTemplates", new[] { "ClientId" });
            DropIndex("dbo.Certificates", new[] { "ClientId" });
            DropIndex("dbo.Certificates", new[] { "TestedBy" });
            DropTable("dbo.KeyValues");
            DropTable("dbo.VolumeTests");
            DropTable("dbo.TemperatureTests");
            DropTable("dbo.PressureTests");
            DropTable("dbo.FrequencyTests");
            DropTable("dbo.VerificationTests");
            DropTable("dbo.Instruments");
            DropTable("dbo.ClientItems");
            DropTable("dbo.ClientCsvTemplates");
            DropTable("dbo.Clients");
            DropTable("dbo.Certificates");
        }
    }
}
