using AutoMapper;
using Prover.Domain.Instrument.Items;
using Prover.Shared.DTO.TestRuns;

namespace Prover.Domain.Verification.TestPoints.SuperFactor
{
    public class SuperFactorTestMappingProfile : Profile
    {
        public SuperFactorTestMappingProfile()
        {
            CreateMap<SuperFactorTestPoint, SuperFactorTestDto>()
                .ForMember(d => d.ItemData, map => map.ResolveUsing(t => t.EvcItems.ItemData));

            CreateMap<SuperFactorTestDto, SuperFactorTestPoint>()
                .ForMember(d => d.EvcItems, map => map.ResolveUsing<ItemGroupResolver<SuperFactorTestDto, SuperFactorTestPoint, ISuperFactorItems>>());
        }
    }
}