using FluentModelBuilder.Alterations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prover.Domain.Model.Verifications;

namespace Prover.Data.EF.Mappers.Verification
{
    public class VerificationRunTestPointMap : IEntityTypeOverride<VerificationRunTestPoint>
    {
        public void Override(EntityTypeBuilder<VerificationRunTestPoint> mapping)
        {
            mapping.ToTable("VerificationRun_TestPoint");
            mapping.HasKey(k => k.Id);
            mapping.Property(p => p.Level);
            mapping.HasOne(tp => tp.VerificationRun)
                .WithMany(v => v.TestPoints)
                .HasForeignKey(tp => tp.VerificationRunId)
                .IsRequired();

            mapping.HasOne(tp => tp.Pressure)
                .WithOne()
                .IsRequired(false);

            mapping.HasOne(tp => tp.Temperature)
                .WithOne()
                .IsRequired(false);

            mapping.HasOne(tp => tp.Volume)
                .WithOne()
                .IsRequired(false);
        }
    }
}