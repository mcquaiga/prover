using AutoMapper;
using Prover.Shared.DTO.Instrument;

namespace Prover.Domain.Instrument
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