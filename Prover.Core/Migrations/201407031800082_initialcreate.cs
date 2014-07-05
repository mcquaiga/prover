namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialcreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Certificates",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false),
                        TestedBy = c.String(),
                        Number = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Instruments",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    SerialNumber = c.Long(nullable: false),
                    TestDateTime = c.DateTime(nullable: false),
                    Type = c.Int(nullable: false),
                    CertificateGuid = c.Guid(nullable: false),
                    Data = c.String(),
                    Temperature_Id = c.Guid(),
                    Volume_Id = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Temperatures", t => t.Temperature_Id)
                .ForeignKey("dbo.Volumes", t => t.Volume_Id)
                .Index(t => t.Temperature_Id)
                .Index(t => t.Volume_Id)
                .Index(t => t.CertificateGuid )
                .Index(t => new {t.SerialNumber, t.TestDateTime, t.Type});
            
            
            CreateTable(
                "dbo.Temperatures",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Data = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TemperatureTests",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TestLevel = c.Int(nullable: false),
                        Gauge = c.Double(nullable: false),
                        PercentError = c.Double(nullable: false),
                        HasPassed = c.Double(nullable: false),
                        Data = c.String(),
                        Temp_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Temperatures", t => t.Temp_Id)
                .Index(t => t.Temp_Id);
            
            CreateTable(
                "dbo.Volumes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PulseACount = c.Int(nullable: false),
                        PulseBCount = c.Int(nullable: false),
                        Data = c.String(),
                    })
                .PrimaryKey(t => t.Id);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Instruments", "Volume_Id", "dbo.Volumes");
            DropForeignKey("dbo.Instruments", "Temperature_Id", "dbo.Temperatures");
            DropForeignKey("dbo.TemperatureTests", "Temp_Id", "dbo.Temperatures");
            DropIndex("dbo.TemperatureTests", new[] { "Temp_Id" });
            DropIndex("dbo.Instruments", new[] { "Volume_Id" });
            DropIndex("dbo.Instruments", new[] { "Temperature_Id" });
            DropTable("dbo.Volumes");
            DropTable("dbo.TemperatureTests");
            DropTable("dbo.Temperatures");
            DropTable("dbo.Instruments");
            DropTable("dbo.Certificates");
        }
    }
}
