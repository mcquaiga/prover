using FluentModelBuilder.Alterations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prover.Domain.Model.Verifications;

namespace Prover.Storage.EF.Mappers.Verification
{
    public class VerificationRunTestPointTemperatureMap : IEntityTypeOverride<TemperatureTest>
    {
        public void Override(EntityTypeBuilder<TemperatureTest> mapping)
        {
            mapping.ToTable("VerificationRun_TestPoint_Temperature");
            mapping.HasKey(p => p.Id);
            //mapping.Property(p => p.GaugeTemperature).IsRequired(true);

            //mapping.Property(p => p.EvcItems).HasColumnName("ItemData");
        }
    }
}