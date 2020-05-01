using Microsoft.Extensions.Logging;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Modules.UnionGas.MasaWebService;
using Prover.Modules.UnionGas.Models;
using Prover.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Prover.Modules.UnionGas.Exporter
{
	/// <summary>
	///     Defines the <see cref="ExportToMasaManager" />
	/// </summary>
	internal class ExportToMasaManager : IExportVerificationTest
	{
		/// <summary>
		///     Defines the Log
		/// </summary>
		private static readonly ILogger Log;

		/// <summary>
		///     Defines the _dcrWebService
		/// </summary>
		private readonly IExportService<QARunEvcTestResult> _exportService;

		private readonly ILogger<ExportToMasaManager> _logger;

		/// <summary>
		///     Defines the _loginService
		/// </summary>
		private readonly ILoginService<Employee> _loginService;

		private readonly IMeterService<MeterDTO> _meterService;

		/// <summary>
		///     Defines the _testRunService
		/// </summary>
		private readonly IVerificationTestService _testRunService;

		public ExportToMasaManager
		(ILogger<ExportToMasaManager> logger, IVerificationTestService testRunService, ILoginService<Employee> loginService, IExportService<QARunEvcTestResult> exportService,
				IMeterService<MeterDTO> meterService
		)
		{
			_logger = logger;
			_testRunService = testRunService;
			_exportService = exportService;
			_meterService = meterService;
			_loginService = loginService;
		}

		public async Task<bool> Export(IEnumerable<EvcVerificationTest> testsForExport)
		{
			var forExport = testsForExport as EvcVerificationTest[] ?? testsForExport.ToArray();
			var qaTestRuns = forExport.Select(Translate.RunTranslationForExport).ToList();
			var isSuccess = await _exportService.SubmitQaTestRunResults(qaTestRuns);

			if (!isSuccess)
			{
				await Notifications.SnackBarMessage.Handle("EXPORT FAILED");
				return false;
				throw new Exception("An error occured sending test results to web service. Please see log for details.");
			}

			await Notifications.SnackBarMessage.Handle("EXPORT SUCCESSFUL");

			foreach (var instr in forExport)
			{
				instr.ExportedDateTime = DateTime.Now;
				await _testRunService.Upsert(instr);
			}

			return true;
		}

		public Task<bool> Export(EvcVerificationTest verificationTest)
		{
			var instrumentList = new List<EvcVerificationTest> { verificationTest };
			return Export(instrumentList);
		}

		public async Task<bool> ExportFailedTest(string companyNumber)
		{
			var meterDto = await _meterService.FindMeterByInventoryNumber(companyNumber);

			if (meterDto == null)
				throw new Exception($"Inventory #{companyNumber} was not be found on an open job.");
			var failedTest = Translate.CreateFailedTestForExport(meterDto, _loginService.User?.UserId);
			return await _exportService.SubmitQaTestRunResults(new[] { failedTest });
		}
	}
}