using FluentModelBuilder.Alterations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prover.Domain.Model.Verifications;

namespace Prover.Data.EF.Mappers.Verification
{
    public class VerificationRunTestPointVolumeMap : IEntityTypeOverride<VolumeTest>
    {
        public void Override(EntityTypeBuilder<VolumeTest> mapping)
        {
            mapping.ToTable("VerificationRun_TestPoint_Volume");
            mapping.HasKey(p => p.Id);
            mapping.Property(p => p.AppliedInput).IsRequired();
            mapping.Property(p => p.PulserA);
            mapping.Property(p => p.PulserB);
        }
    }
}