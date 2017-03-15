using Prover.Shared.DTO.TestRuns;

namespace Prover.Domain.Models.TestRuns
{
    public class TestRunFactory
    {
        public TestRun Create(TestRunDto testRunDto)
        {
            return new TestRun();
            //var instrumentType = CommProtocol.Common.InstrumentTypes.Instruments.GetAll().FirstOrDefault(i => i.Id == testRunDto.InstrumentType.Id);

            //var itemValues = _testRunDto.ItemValues.ToDictionary(x => int.Parse(x.Id), y => y.Value);
            //ItemValues = ItemHelpers.LoadItems(InstrumentType, itemValues).ToList();
        }
    }
}