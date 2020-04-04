using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using Prover.Modules.UnionGas.DcrWebService;

namespace Prover.Modules.UnionGas.MasaWebService
{
    public interface IMeterService<TMeter>
    {
        Task<TMeter> FindMeterByInventoryNumber(string inventoryNumber);
        Task<IList<TMeter>> GetOutstandingMeterTestsByJobNumber(int jobId);
    }

    public interface IExportService<in TResult>
    {
        Task<bool> SubmitQaTestRunResults(IEnumerable<TResult> evcQaRuns);
    }

    public class MasaService : 
        IUserService<EmployeeDTO>, 
        IMeterService<MeterDTO>, 
        IExportService<QARunEvcTestResult>
    {
        private readonly ILogger<MasaService> _logger;
        private readonly DCRWebServiceSoap _webService;

        public MasaService(ILogger<MasaService> logger, DCRWebServiceSoap webService)
        {
            _logger = logger;
            _webService = webService;
        }

        public async Task<MeterDTO> FindMeterByInventoryNumber(string inventoryNumber)
        {
            try
            {
                return await _webService.FindMeterByInventoryNumber(inventoryNumber, _logger);
            }
            catch (EndpointNotFoundException ex)
            {
                _logger.LogWarning(ex, "An error occured while contacting the MASA web service. ");
                return null;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, "An error occured while contacting the MASA web service. ");
                return null;
            }
        }

        public async Task<IList<MeterDTO>> GetOutstandingMeterTestsByJobNumber(int jobId)
        {
            return await _webService.GetOutstandingMeterTestsByJobNumber(jobId, _logger);
        }

        public async Task<bool> SubmitQaTestRunResults(IEnumerable<QARunEvcTestResult> evcQaRuns)
        {
            return await _webService.SendQaTestResults(evcQaRuns, _logger);
        }

        public async Task<EmployeeDTO> GetUser(string employeeNumber)
        {
            if (string.IsNullOrEmpty(employeeNumber)) return null;

            _logger.LogDebug($"Requesting employee with #{employeeNumber} from MASA.");

            var employeeRequest = new GetEmployeeRequest(new GetEmployeeRequestBody(employeeNumber));
            var response = await _webService.GetEmployeeAsync(employeeRequest);

            return response.Body.GetEmployeeResult;

        }
    }
}