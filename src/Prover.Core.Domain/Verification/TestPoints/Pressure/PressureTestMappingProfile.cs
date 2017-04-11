using AutoMapper;
using Prover.Domain.Instrument.Items;
using Prover.Shared.DTO.TestRuns;

namespace Prover.Domain.Verification.TestPoints.Pressure
{
    public class PressureTestMappingProfile : Profile
    {
        public PressureTestMappingProfile()
        {
            CreateMap<PressureTestPoint, PressureTestDto>()
                .ForMember(d => d.ItemData, map => map.ResolveUsing(t => t.EvcItems.ItemData));

            CreateMap<PressureTestDto, PressureTestPoint>()
                .ForMember(d => d.EvcItems, map => map.ResolveUsing<ItemGroupResolver<PressureTestDto, PressureTestPoint, IPressureItems>>());
        }
    }
}