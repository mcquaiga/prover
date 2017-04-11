using AutoMapper;
using Prover.Shared.DTO.TestRuns;

namespace Prover.Domain.Verification.TestRun
{
    public class TestRunMappingProfile : Profile
    {
        public TestRunMappingProfile()
        {
            CreateMap<TestRunDto, TestRun>();
            CreateMap<TestRun, TestRunDto>();
        }
    }
}