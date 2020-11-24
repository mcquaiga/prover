using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using UnionGas.MASA.DCRWebService;

namespace UnionGas.MASA {

	/// <summary>
	/// Defines the <see cref="DCRWebServiceCommunicator"/>
	/// </summary>
	public class DCRWebServiceCommunicator {

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DCRWebServiceCommunicator"/> class.
		/// </summary>
		/// <param name="dcrWebService">The dcrWebService <see cref="DCRWebServiceSoap"/></param>
		public DCRWebServiceCommunicator(DCRWebServiceSoap dcrWebService) {
			_dcrWebService = dcrWebService;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the SoapClient
		/// </summary>
		public DCRWebServiceSoapClient SoapClient => (DCRWebServiceSoapClient)_dcrWebService;

		#endregion

		#region Fields

		/// <summary>
		/// Defines the _dcrWebService
		/// </summary>
		private readonly DCRWebServiceSoap _dcrWebService;

		/// <summary>
		/// Defines the _log
		/// </summary>
		private readonly Logger _log = LogManager.GetCurrentClassLogger();

		#endregion

		#region Methods

		/// <summary>
		/// The FindMeterByCompanyNumber
		/// </summary>
		/// <param name="companyNumber">The companyNumber <see cref="string"/></param>
		/// <returns>The <see cref="Task{MeterDTO}"/></returns>
		public async Task<MeterDTO> FindMeterByCompanyNumber(string companyNumber) {
			if (string.IsNullOrEmpty(companyNumber))
				throw new ArgumentNullException(nameof(companyNumber));

			_log.Debug($"Finding meter with inventory number {companyNumber} in MASA.");

			var request = new GetValidatedEvcDeviceByBarcodeRequest {
				Body = new GetValidatedEvcDeviceByBarcodeRequestBody(companyNumber)
			};



			var response =
				await CallWebServiceMethod(() => _dcrWebService.GetValidatedEvcDeviceByBarcodeAsync(request))
					.ConfigureAwait(false);

			return response.Body.GetValidatedEvcDeviceByBarcodeResult;
		}

		/// The FindMeterByCompanyNumber
		/// </summary>
		/// <param name="companyNumber">The companyNumber <see cref="string"/></param>
		/// <returns>The <see cref="Task{MeterDTO}"/></returns>
		public async Task<MeterDTO> FindMeterByBarCodeNumber(string barCodeNumber) {
			if (string.IsNullOrEmpty(barCodeNumber))
				throw new ArgumentNullException(nameof(barCodeNumber));

			_log.Debug($"Finding meter with bar code number {barCodeNumber}.");

			var request = new GetValidatedEvcDeviceByBarcodeRequest {
				Body = new GetValidatedEvcDeviceByBarcodeRequestBody(barCodeNumber)
			};

			var response =
				await CallWebServiceMethod(() => _dcrWebService.GetValidatedEvcDeviceByBarcodeAsync(request))
					.ConfigureAwait(false);

			return response.Body.GetValidatedEvcDeviceByBarcodeResult;
		}

		/// <summary>
		/// The GetEmployee
		/// </summary>
		/// <param name="username">The username <see cref="string"/></param>
		/// <returns>The <see cref="Task{EmployeeDTO}"/></returns>
		public async Task<EmployeeDTO> GetEmployee(string username) {
			_log.Debug($"Getting employee with #{username} from MASA.");

			var employeeRequest = new GetEmployeeRequest(new GetEmployeeRequestBody(username));
			var response = await CallWebServiceMethod(() => _dcrWebService.GetEmployeeAsync(employeeRequest))
				.ConfigureAwait(false);

			return response.Body.GetEmployeeResult;
		}

		/// <summary>
		/// The GetOutstandingMeterTestsByJobNumber
		/// </summary>
		/// <param name="jobNumber">The jobNumber <see cref="int"/></param>
		/// <returns>The <see cref="Task{IList{MeterDTO}}"/></returns>
		public async Task<IList<MeterDTO>> GetOutstandingMeterTestsByJobNumber(int jobNumber) {
			var request = new GetMeterListByJobNumberRequest(new GetMeterListByJobNumberRequestBody(jobNumber.ToString()));

			var response = await CallWebServiceMethod(() => _dcrWebService.GetMeterListByJobNumberAsync(request))
				.ConfigureAwait(false);

			return response.Body.GetMeterListByJobNumberResult.ToList();
		}

		/// <summary>
		/// The SendResultsToWebService
		/// </summary>
		/// <param name="evcQaRuns">The evcQaRuns <see cref="IEnumerable{QARunEvcTestResult}"/></param>
		/// <returns>The <see cref="Task{bool}"/></returns>
		public async Task<bool> SendQaTestResults(ICollection<QARunEvcTestResult> evcQaRuns) {
			if (evcQaRuns == null || !evcQaRuns.Any())
				throw new ArgumentOutOfRangeException(nameof(evcQaRuns));

			var request = new SubmitQAEvcTestResultsRequest(
				new SubmitQAEvcTestResultsRequestBody(evcQaRuns.ToArray())
			);

			var response = await CallWebServiceMethod(() =>
					_dcrWebService.SubmitQAEvcTestResultsAsync(request))
				.ConfigureAwait(false);

			if (string.Equals(response.Body.SubmitQAEvcTestResultsResult, "success",
				StringComparison.OrdinalIgnoreCase)) {
				_log.Info("Web service returned successfully!");
				return true;
			}

			_log.Error($"Web service return an error: {Environment.NewLine} {response.Body}");
			return false;
		}

		/// <summary>
		/// The CallWebServiceMethod
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="webServiceMethod">The webServiceMethod <see cref="Func{Task{TResult}}"/></param>
		/// <returns>The <see cref="Task{TResult}"/></returns>
		private async Task<TResult> CallWebServiceMethod<TResult>(Func<Task<TResult>> webServiceMethod, CancellationTokenSource tokenSource = null) {
			try {
				if (tokenSource == null)
					tokenSource = new CancellationTokenSource(new TimeSpan(0, 0, 0, 3));

				tokenSource.Token.ThrowIfCancellationRequested();

				return await Task.Run(async () => await webServiceMethod.Invoke(), tokenSource.Token)
					.ConfigureAwait(false);
			}
			catch (OperationCanceledException) {
				_log.Warn("Timed out contacting the web service.");
				throw;
			}
			catch (EndpointNotFoundException ex) {
				var msg =
					$"MASA Web service could not be reached. {Environment.NewLine} " +
					$"Endpoint: {SoapClient.Endpoint.Address} {Environment.NewLine}";

				_log.Error(ex, msg);

				throw;
			}
			catch (Exception ex) {
				_log.Error(ex, "An error occured contacting the web service.");
				throw;
			}
		}

		#endregion
	}
}