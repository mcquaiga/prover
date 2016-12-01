using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.ExternalIntegrations.Validators;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using UnionGas.MASA.DCRWebService;

namespace UnionGas.MASA.Validators
{
    //public class UnionGasItemValidatorUpdater
    //{
    //    private readonly Logger _log = LogManager.GetCurrentClassLogger();
    //    private readonly IInstrumentStore<Instrument> _instrumentStore;
    //    private readonly LoginService _loginService;
    //    private readonly DCRWebServiceSoap _webService;
    //    private readonly IGetValue _valueGetter;

    //    public UnionGasItemValidatorUpdater(IInstrumentStore<Instrument> instrumentStore, LoginService loginService, DCRWebServiceSoap webService, IGetValue valueGetter)
    //    {
    //        _instrumentStore = instrumentStore;
    //        _loginService = loginService;
    //        _webService = webService;
    //        _valueGetter = valueGetter;
    //    }

    //    public async Task ValidateAndUpdate(EvcCommunicationClient commClient, Instrument instrument)
    //    {
    //        var meterDto = await Validate(commClient, instrument);

    //        //companyNumber = (string)await Updater.Update(commClient, instrument);
    //    }

    //    public async Task<object> Validate(EvcCommunicationClient commClient, Instrument instrument)
    //    {
    //        var companyNumberItem = instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber);
    //        var companyNumber = companyNumberItem.RawValue;

    //        MeterDTO meterDto;
    //        do
    //        {
    //            meterDto = await VerifyWithWebService(companyNumber);

    //            if (meterDto?.InventoryCode == null || meterDto.InventoryCode != companyNumber)
    //            {
    //                _log.Warn($"Inventory code {companyNumber} not found in an open job.");
    //                companyNumber = _valueGetter.GetValue();
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        } while (companyNumber != null);

    //        return meterDto;
    //    }

    //    public async Task<MeterDTO> VerifyWithWebService(string companyNumber)
    //    {
    //        _log.Debug($"Verifying company number {companyNumber} with web service.");

    //        try
    //        {
    //            var request = new GetValidatedEvcDeviceByInventoryCodeRequest
    //            {
    //                Body = new GetValidatedEvcDeviceByInventoryCodeRequestBody(companyNumber)
    //            };

    //            var response = await WebService.GetValidatedEvcDeviceByInventoryCodeAsync(request);
    //            return response.Body.GetValidatedEvcDeviceByInventoryCodeResult;
    //        }
    //        catch (EndpointNotFoundException)
    //        {
    //            _log.Warn($"Web service not available. Skipping company number verification.");
    //            return null;
    //        }
    //    }
    //}
}
