using Prover.Domain.Instrument.Items;
using Prover.Domain.Verification.TestPoints.Pressure;
using Prover.Services.Mappings.Resolvers;
using Prover.Shared.DTO.TestRuns;

namespace Prover.Services.Mappings.Profiles
{
    public class PressureTestMappingProfile : Profile
    {
        public PressureTestMappingProfile()
        {
            CreateMap<PressureTestPoint, PressureTestDto>()
                .ForMember(d => d.ItemData, map => map.ResolveUsing(t => t.EvcItems.ItemData));

            CreateMap<PressureTestDto, PressureTestPoint>()
                .ForMember(d => d.EvcItems,
                    map => map.ResolveUsing<ItemGroupResolver<PressureTestDto, PressureTestPoint, IPressureItems>>());
        }
    }
}