using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prover.Application.Services
{
	public class ExportManager : IExportVerificationTest
	{
		private readonly IVerificationService _testRunService;

		public ExportManager(IVerificationService testRunService)
		{
			_testRunService = testRunService;
		}

		public async Task<bool> Export(EvcVerificationTest verificationTest)
		{
			verificationTest.ExportedDateTime = DateTime.Now;
			await _testRunService.Save(verificationTest);
			return true;
		}

		public async Task<bool> Export(IEnumerable<EvcVerificationTest> verificationTests)
		{
			foreach (var test in verificationTests)
			{
				test.ExportedDateTime = DateTime.Now;
				await _testRunService.Save(test);
			}
			return true;
		}
	}
}
