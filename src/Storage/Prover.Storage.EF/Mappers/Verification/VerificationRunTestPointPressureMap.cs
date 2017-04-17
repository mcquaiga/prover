using FluentModelBuilder.Alterations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prover.Domain.Model.Verifications;

namespace Prover.Storage.EF.Mappers.Verification
{
    public class VerificationRunTestPointPressureMap : IEntityTypeOverride<PressureTest>
    {
        public void Override(EntityTypeBuilder<PressureTest> mapping)
        {
            mapping.ToTable("VerificationRun_TestPoint_Pressure");
            mapping.HasKey(p => p.Id);
            //mapping.Property(p => p.AtmosphericPressure).HasDefaultValue(0);
            //mapping.Property(p => p.GaugePressure).IsRequired();
        }
    }
}