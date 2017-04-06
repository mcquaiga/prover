using System;
using System.Collections.Generic;
using AutoMapper;
using Prover.Domain.Models.Instruments;
using Prover.Shared.Common;
using Prover.Shared.DTO.TestRuns;
using Prover.Shared.Enums;

namespace Prover.Domain.Models.VerificationTests
{
    public interface IInstrumentFactory
    {
        IInstrument Create(string instrumentType);
    }

    public class TestRunFactory
    {
        private readonly IInstrumentFactory _instrumentFactory;

        public TestRunFactory(IInstrumentFactory instrumentFactory)
        {
            _instrumentFactory = instrumentFactory;
        }

        public static TestRun Create(IInstrument instrument)
        {
            var testRun = new TestRun
            {
                Instrument = instrument,
                TestDateTime = DateTime.Now,
                ExportedDateTime = null,
                ArchivedDateTime = null,
                TestPoints = new Dictionary<TestLevel, TestPoint>()
            };

            foreach (TestLevel testLevel in Enum.GetValues(typeof(TestLevel)))
            {
                var testPoint = TestPoint.Create(testLevel, instrument);
                testRun.TestPoints.Add(testLevel, testPoint);
            }

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

    public class TestRun : Entity
    {
        public IInstrument Instrument { get; set; }
        public DateTime TestDateTime { get; set; }
        public DateTime? ExportedDateTime { get; set; }
        public DateTime? ArchivedDateTime { get; set; }
        public bool? EventLogPassed { get; set; }
        public bool? CommPortsPassed { get; set; }
        public string EmployeeId { get; set; }
        public string JobId { get; set; }

        public Dictionary<TestLevel, TestPoint> TestPoints { get; set; }
    }
}