using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Prover.Domain.Instrument;
using Prover.Domain.Verification.TestPoints;
using Prover.Shared.DTO.TestRuns;
using Prover.Shared.Enums;

namespace Prover.Domain.Verification.TestRun
{
    public class TestRunFactory
    {
        public static async Task<TestRun> Create(IInstrumentFactory instrumentFactory)
        {
            var instrument = await instrumentFactory.Create();

            var testRun = new TestRun(instrument, null);

            return testRun;
        }

        public static TestRun Create(TestRunDto testRunDto)
        {
            var testRun = Mapper.Map<TestRun>(testRunDto);

            return testRun;
        }

        public static TestRunDto Create(TestRun testRun)
        {
            var testRunDto = Mapper.Map<TestRunDto>(testRun);

            return testRunDto;
        }
    }
}