using NLog;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnionGas.MASA.DCRWebService;

namespace UnionGas.MASA
{
    public class DCRWebServiceCommunicator
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly DCRWebServiceSoap _dcrWebService;

        public DCRWebServiceCommunicator(DCRWebServiceSoap dcrWebService)
        {
            _dcrWebService = dcrWebService;
        }

        public async Task<IList<MeterDTO>> GetOutstandingMeterTestsByJobNumber(int jobNumber)
        {
            var request = new GetMeterListByJobNumberRequest(new GetMeterListByJobNumberRequestBody(jobNumber));

            var response = await _dcrWebService.GetMeterListByJobNumberAsync(request);
            
            return response.Body.GetMeterListByJobNumberResult.ToList();
        }
    }
}
