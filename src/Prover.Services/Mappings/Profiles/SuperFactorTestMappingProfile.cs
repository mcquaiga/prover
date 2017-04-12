namespace Prover.Services.Mappings.Profiles
{
    public class SuperFactorTestMappingProfile : Profile
    {
        public SuperFactorTestMappingProfile()
        {
            CreateMap<SuperFactorTestPoint, SuperFactorTestDto>()
                .ForMember(d => d.ItemData, map => map.ResolveUsing(t => t.EvcItems.ItemData));

            CreateMap<SuperFactorTestDto, SuperFactorTestPoint>()
                .ForMember(d => d.EvcItems,
                    map =>
                        map.ResolveUsing<ItemGroupResolver<SuperFactorTestDto, SuperFactorTestPoint, ISuperFactorItems>>
                            ());
        }
    }
}