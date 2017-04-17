namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveCertificates : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Instruments", "CertificateId", "dbo.Certificates");
            DropIndex("dbo.Instruments", new[] { "CertificateId" });
            DropColumn("dbo.Instruments", "CertificateId");
            DropTable("dbo.Certificates");
        }
        
        public override void Down()
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
            
            AddColumn("dbo.Instruments", "CertificateId", c => c.Guid());
            CreateIndex("dbo.Instruments", "CertificateId");
            AddForeignKey("dbo.Instruments", "CertificateId", "dbo.Certificates", "Id");
        }
    }
}
