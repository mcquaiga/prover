using FluentModelBuilder.Alterations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prover.Domain.Model.Verifications;

namespace Prover.Data.EF.Mappers.Verification
{
    public class VerificationRunMap : IEntityTypeOverride<VerificationRun>
    {
        public void Override(EntityTypeBuilder<VerificationRun> mapping)
        {
            mapping.ToTable("VerificationRun");
            mapping.HasKey(k => k.Id);

            mapping.HasOne(t => t.Instrument)
                .WithOne();

            mapping.HasMany(t => t.TestPoints)
                .WithOne(p => p.VerificationRun);
        }
    }
}
