namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Indexes : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Certificates", new[] { "ClientId" });
            DropIndex("dbo.Instruments", new[] { "CertificateId" });
            DropIndex("dbo.Instruments", new[] { "ClientId" });
            CreateIndex("dbo.Certificates", "TestedBy");
            CreateIndex("dbo.Certificates", "ClientId");
            CreateIndex("dbo.Instruments", "ArchivedDateTime");
            CreateIndex("dbo.Instruments", "CertificateId");
            CreateIndex("dbo.Instruments", "ClientId");
            CreateIndex("dbo.Instruments", "ExportedDateTime");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Instruments", new[] { "ExportedDateTime" });
            DropIndex("dbo.Instruments", new[] { "ClientId" });
            DropIndex("dbo.Instruments", new[] { "CertificateId" });
            DropIndex("dbo.Instruments", new[] { "ArchivedDateTime" });
            DropIndex("dbo.Certificates", new[] { "ClientId" });
            DropIndex("dbo.Certificates", new[] { "TestedBy" });
            CreateIndex("dbo.Instruments", "ClientId");
            CreateIndex("dbo.Instruments", "CertificateId");
            CreateIndex("dbo.Certificates", "ClientId");
        }
    }
}
