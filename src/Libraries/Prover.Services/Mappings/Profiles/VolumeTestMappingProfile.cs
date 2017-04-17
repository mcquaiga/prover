using AutoMapper;
using Prover.Services.Mappings.Resolvers;
using Prover.Shared.DTO.TestRuns;

namespace Prover.Services.Mappings.Profiles
{
    public class VolumeTestMappingProfile : Profile
    {
        public VolumeTestMappingProfile()
        {
            //CreateMap<VolumeTestPoint, VolumeTestDto>()
            //    .ForMember(d => d.ItemData, map => map.ResolveUsing(t => t.EvcItems.ItemData))
            //    .ForMember(d => d.PreTestItemData, map => map.ResolveUsing(t => t.PreTestItems.ItemData))
            //    .ForMember(d => d.PostTestItemData, map => map.ResolveUsing(t => t.PostTestItems.ItemData));

            //CreateMap<VolumeTestDto, VolumeTestPoint>()
            //    .ForMember(d => d.DriveType, map => map.Ignore())
            //    .ForMember(d => d.EvcItems,
            //        map => map.ResolveUsing<ItemGroupResolver<VolumeTestDto, VolumeTestPoint, IVolumeItems>>())
            //    .ForMember(d => d.PreTestItems,
            //        map =>
            //            map.ResolveUsing(
            //                new ItemGroupResolver<VolumeTestDto, VolumeTestPoint, IVolumeItems>(v => v.PreTestItemData)))
            //    .ForMember(d => d.PostTestItems,
            //        map =>
            //            map.ResolveUsing(
            //                new ItemGroupResolver<VolumeTestDto, VolumeTestPoint, IVolumeItems>(v => v.PostTestItemData)))
            //    .AfterMap((dto, point) => point.SetDriveType())
            //    .AfterMap((source, dest) => { });
        }
    }
}