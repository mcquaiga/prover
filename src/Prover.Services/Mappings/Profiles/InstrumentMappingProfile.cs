using AutoMapper;
using Prover.Domain.Instrument;
using Prover.Services.Mappings.Convertors;
using Prover.Shared.DTO.Instrument;

namespace Prover.Services.Mappings.Profiles
{
    public class InstrumentMappingProfile : Profile
    {
        public InstrumentMappingProfile()
        {
            CreateMap<IInstrument, InstrumentDto>()
                .ForMember(d => d.InstrumentFactory, map => map.ResolveUsing(t => t.InstrumentFactory.TypeString))
                .ForMember(d => d.ItemData, map => map.ResolveUsing(t => t.ItemData));

            CreateMap<InstrumentDto, IInstrument>()
                .ConvertUsing<InstrumentObjectConverter>();
        }
    }
}