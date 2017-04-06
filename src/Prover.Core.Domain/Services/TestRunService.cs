using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;

namespace Prover.Domain.Services
{
    //public class TestRunService
    //{
    //    private readonly IRepository<TestRunDto> _testRunRepository;

    //    public TestRunService(IRepository<TestRunDto> testRunRepository)
    //    {
    //        _testRunRepository = testRunRepository;
    //    }

    //    public IEnumerable<TestRun> GetAllNotExported()
    //    {
    //        return _testRunRepository.Query()
    //            .Where(t => t.ExportedDateTime != null)
    //            .ProjectTo<TestRun>().ToList();
    //    }

    //    public async Task<TestRun> Get(Guid id)
    //    {
    //        var testRunDto = await _testRunRepository.Get(id);
    //        return Mapper.Map<TestRun>(testRunDto);
    //    }

    //    public async Task<bool> Add(TestRun testRun)
    //    {
    //        var testRunDto = Mapper.Map<TestRunDto>(testRun);
    //        var result = await _testRunRepository.UpsertAsync(testRunDto);
    //        return result != null;
    //    }

    //    public async Task Remove(TestRun testRun)
    //    {
    //        var testRunDto = Mapper.Map<TestRunDto>(testRun);
    //        await _testRunRepository.Delete(testRunDto);
    //    }
    //}
}