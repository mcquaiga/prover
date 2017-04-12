namespace Prover.Services.Mappings.Profiles
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