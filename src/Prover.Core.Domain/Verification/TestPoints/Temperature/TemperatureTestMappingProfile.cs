using AutoMapper;
using Prover.Domain.Instrument.Items;
using Prover.Shared.DTO.TestRuns;

namespace Prover.Domain.Verification.TestPoints.Temperature
{
    public class TemperatureTestMappingProfile : Profile
    {
        public TemperatureTestMappingProfile()
        {
            CreateMap<TemperatureTestPoint, TemperatureTestDto>()
                .ForMember(d => d.ItemData, map => map.ResolveUsing(t => t.EvcItems.ItemData));

            CreateMap<TemperatureTestDto, TemperatureTestPoint>()
                .ForMember(d => d.EvcItems, map => map.ResolveUsing<ItemGroupResolver<TemperatureTestDto, TemperatureTestPoint, ITemperatureItems>>());
        }
    }
}