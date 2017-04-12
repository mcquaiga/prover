using System.Collections.Generic;
using AutoMapper;
using Newtonsoft.Json;
using Prover.Data.EF.Models.TestRun;
using Prover.Shared.DTO.Instrument;
using Prover.Shared.DTO.TestRuns;

namespace Prover.Data.EF.Mappers.Resolvers
{
    internal class InstrumentDtoResolver : IValueResolver<TestRunDatabase, TestRunDto, InstrumentDto>
    {
        public InstrumentDto Resolve(TestRunDatabase source, TestRunDto destination, InstrumentDto destMember, ResolutionContext context)
        {
            return new InstrumentDto()
            {
                InstrumentFactory = source.Instrument,
                ItemData = JsonConvert.DeserializeObject<Dictionary<string, string>>(source.ItemData)
            };
        }
    }
}
