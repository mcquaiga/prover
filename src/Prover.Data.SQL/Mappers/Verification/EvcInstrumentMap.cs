using FluentModelBuilder.Alterations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prover.Domain.Model.Instrument;

namespace Prover.Data.EF.Mappers.Verification
{
    public class EvcInstrumentMap : IEntityTypeOverride<EvcInstrument>
    {
        public void Override(EntityTypeBuilder<EvcInstrument> mapping)
        {
            mapping.ToTable("Instrument");
            mapping.HasKey(k => k.Id);
            mapping.Property(p => p.ItemsSerialized).HasColumnName("Items")
                .UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);
            mapping.Ignore(p => p.Items);

        }
    }
}