using System.Collections.Generic;
using AutoMapper;
using Prover.Data.EF.Mappers.Resolvers;
using Prover.Shared.DTO.TestRuns;

namespace Prover.Data.EF.Models.TestRun
{
    static class TestRunMapping
    {
        public static void Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<TestRunDto, TestRunDao>();
               // .ForMember(x => x.ItemData, map => map.ResolveUsing(new SerializeResolver<TestRunDto, Dictionary<string, string>, TestRunDao>(x => x.ItemData)));
                
              cfg.CreateMap<TestRunDao, TestRunDto>();
                //.ForMember(x => x.ItemData, map => map.ResolveUsing(new DeserializeResolver<TestRunDao, string, TestRunDto, Dictionary<string, string>>(x => x.ItemData)));
        }
    }
}
