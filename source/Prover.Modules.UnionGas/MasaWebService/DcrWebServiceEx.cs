using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Modules.UnionGas.DcrWebService;

namespace Prover.Modules.UnionGas.MasaWebService
{
    public static class DcrWebServiceEx
    {
        public static async Task<MeterDTO> FindMeterByCompanyNumber(this DCRWebServiceSoap webService,
            string companyNumber, ILogger logger = null)
        {
            logger ??= NullLogger.Instance;

            logger.LogDebug($"Finding meter with inventory number {companyNumber} in MASA.");

            var request = new GetValidatedEvcDeviceByInventoryCodeRequest
            {
                Body = new GetValidatedEvcDeviceByInventoryCodeRequestBody(companyNumber)
            };

            var response = await CallWebServiceMethod(webService,
                    () => webService.GetValidatedEvcDeviceByInventoryCodeAsync(request),
                    logger)
                .ConfigureAwait(false);
            return response.Body.GetValidatedEvcDeviceByInventoryCodeResult;
        }

        public static async Task<IList<MeterDTO>> GetOutstandingMeterTestsByJobNumber(this DCRWebServiceSoap webService, int jobNumber, ILogger logger = null)
        {
            logger ??= NullLogger.Instance;

            var request = new GetMeterListByJobNumberRequest(new GetMeterListByJobNumberRequestBody(jobNumber));

            var response = await CallWebServiceMethod(webService, () => webService.GetMeterListByJobNumberAsync(request), logger).ConfigureAwait(false);

            return response.Body.GetMeterListByJobNumberResult.ToList();
        }

        public static async Task<bool> SendQaTestResults(this DCRWebServiceSoap webService, IEnumerable<QARunEvcTestResult> evcQaRuns, ILogger logger = null)
        {
            logger ??= NullLogger.Instance;
            var items = evcQaRuns.ToArray();

            if (items.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(evcQaRuns));

            var request = new SubmitQAEvcTestResultsRequest(new SubmitQAEvcTestResultsRequestBody(items));

            var response = await CallWebServiceMethod(webService, () => webService.SubmitQAEvcTestResultsAsync(request), logger)
                .ConfigureAwait(false);

            var result = response.Body.SubmitQAEvcTestResultsResult;

            if (!string.Equals(result, "success", StringComparison.OrdinalIgnoreCase)) logger.LogError($"Web service return an error: {Environment.NewLine} {result}");

            return string.Equals(result, "success", StringComparison.OrdinalIgnoreCase);
        }

        private static async Task<TResult> CallWebServiceMethod<TResult>(DCRWebServiceSoap webService,
            Func<Task<TResult>> webServiceMethod, ILogger log = null)
        {
            log ??= NullLogger.Instance;
            try
            {
                var tokenSource = new CancellationTokenSource(new TimeSpan(0, 0, 0, 3));
                tokenSource.Token.ThrowIfCancellationRequested();

                return await Task.Run(async () => await webServiceMethod.Invoke(), tokenSource.Token)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                log.LogWarning("Timed out contacting the web service.");
                throw;
            }

            catch (EndpointNotFoundException ex)
            {
                log.LogError(ex, $"MASA Web service could not be reached. {Environment.NewLine}" +
                                 $" Endpoint: {(webService as DCRWebServiceSoapClient)?.Endpoint.Address} {Environment.NewLine}" +
                                 $" State: {(webService as DCRWebServiceSoapClient)?.State.ToString()}");
                throw;
            }

            catch (Exception ex)
            {
                log.LogError(ex, "An error occured contacting the web service.");
                throw;
            }
        }
    }
}