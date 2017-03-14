using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.InstrumentTypes;
using Prover.Domain.Models.TestRuns;
using Prover.Domain.Services;
using Prover.Shared.DTO.Instrument;
using Prover.Shared.DTO.TestRuns;
using Prover.Shared.Enums;
using Prover.Shared.Storage;

namespace Prover.Tests.Domain.Services
{
    [TestClass]
    public class TestRunServiceTests
    {
        private MockRepository mockRepository;

        private Mock<IRepository<TestRunDto>> _mockRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            _mockRepository = mockRepository.Create<IRepository<TestRunDto>>();
        }

        private TestRunDto CreateTestRun()
        {
            var items = 

            return new TestRunDto(EvcCorrectorType.PTZ, null, null, true, true, string.Empty, string.Empty, 
                EmployeeId = string.Empty,
                EventLogPassed = true,
                ExportedDateTime = null,
                InstrumentType = new InstrumentTypeDto(),
                ItemValues = new List<EvcItemValueDto>()
                {
                    new EvcItemValueDto {Id = "62", Value = "123456"},
                    new EvcItemValueDto {Id = "201", Value = "987654321"},
                }
            };
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Add_TestRun()
        {
            TestRunService service = this.CreateService();

        }

        private TestRunService CreateService()
        {
            return new TestRunService(
                _mockRepository.Object);
        }
    }
}