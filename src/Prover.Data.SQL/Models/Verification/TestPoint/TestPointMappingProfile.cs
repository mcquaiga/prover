using System.Collections.Generic;
using AutoMapper;
using Newtonsoft.Json;
using Prover.Data.EF.Mappers;
using Prover.Data.EF.Mappers.Resolvers;
using Prover.Data.EF.Models.PressureTest;
using Prover.Data.EF.Models.TestRun;
using Prover.Shared.DTO.TestRuns;

namespace Prover.Data.EF.Models.TestPoint
{
    internal class TestPointMappingProfile : Profile
    {
        public TestPointMappingProfile()
        {
            CreateMap<Dictionary<string, string>, string>()
                .ConvertUsing<SerializeConverter>();
            
            CreateMap<string, Dictionary<string, string>>()
                .ConvertUsing<DeserializeConverter<Dictionary<string, string>>>();

            CreateMap<TestPointDto, TestPointDatabase>();
                
            CreateMap<TestPointDatabase, TestPointDto>()
                .ForMember(d => d.SuperFactor, map => map.Ignore());
        }
    }

    internal class PressureTestMappingProfile : Profile
    {
        public PressureTestMappingProfile()
        {
            CreateMap<PressureTestDto, PressureTestDatabase>();

            CreateMap<PressureTestDatabase, PressureTestDto>();
        }
    }

    internal class TemperatureTestMappingProfile : Profile
    {
        public TemperatureTestMappingProfile()
        {
            CreateMap<TemperatureTestDto, TemperatureTestDatabase>();

            CreateMap<TemperatureTestDatabase, TemperatureTestDto>();
        }
    }

    internal class VolumeTestMappingProfile : Profile
    {
        public VolumeTestMappingProfile()
        {
            CreateMap<VolumeTestDto, VolumeTestDatabase>();

            CreateMap<VolumeTestDatabase, VolumeTestDto>()
                .ForMember(d => d.ItemData, map => map.Ignore());
        }
    }
}
