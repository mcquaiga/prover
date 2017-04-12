using System.Collections.Generic;
using AutoMapper;
using Prover.Data.EF.Mappers.Resolvers;
using Prover.Shared.DTO.TestRuns;

namespace Prover.Data.EF.Models.TestRun
{
    public class TestRunMappingProfile : Profile
    {
        public TestRunMappingProfile()
        {
            CreateMap<TestRunDto, TestRunDatabase>()
                .ForMember(x => x.ItemData, map => map.ResolveUsing(new SerializeResolver<TestRunDto, Dictionary<string, string>, TestRunDatabase>(x => x.Instrument.ItemData)));

            CreateMap<TestRunDatabase, TestRunDto>()
                .ForMember(d => d.Instrument, map => map.ResolveUsing<InstrumentDtoResolver>());
        }
    }
}
