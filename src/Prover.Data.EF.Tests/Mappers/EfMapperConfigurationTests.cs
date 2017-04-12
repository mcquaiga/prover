using AutoMapper;
using NUnit.Framework;
using Prover.Data.EF.Mappers;

namespace Prover.Data.EF.Tests.Mappers
{
    [TestFixture()]
    public class EfMapperConfigurationTests
    {
        [Test]
        public void Test_Mapping_Config()
        {
            Mapper.Initialize(EfMapperConfiguration.Configure);
            Mapper.AssertConfigurationIsValid();
        }
    }
}