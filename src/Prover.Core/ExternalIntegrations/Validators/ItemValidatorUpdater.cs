using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.Core.Models.Instruments;

namespace Prover.Core.ExternalIntegrations.Validators
{
    public interface IValidator
    {
        Task<object> Validate(EvcCommunicationClient evcCommunicationClient, Instrument instrument);
    }

    public interface IUpdater
    {
        Task<object> Update(EvcCommunicationClient evcCommunicationClient, Instrument instrument, CancellationToken ct);
    }

    public interface IGetValue
    {
        string GetValue();
    }

    //public abstract class ItemValidatorUpdater
    //{
    //    private readonly IValidator _validator;
    //    private readonly IUpdater _updater;
    //    private readonly IGetValue _getter;

    //    protected ItemValidatorUpdater(IValidator validator, IUpdater updater, IGetValue getter)
    //    {
    //        _validator = validator;
    //        _updater = updater;
    //        _getter = getter;
    //    }

    //    public async Task Run(EvcCommunicationClient commClient, Instrument instrument)
    //    {
    //        bool isValid = false;

    //        do
    //        {
    //            isValid = await _validator.Validate(commClient, instrument);

    //            if (!isValid)
    //            {
                    
    //            }
    //        } while (!isValid);

    //        if (!isValid)
    //        {

    //        }
    //    }

    //    public abstract Task<bool> Update(EvcCommunicationClient evcCommunicationClient, Instrument instrument);
    //    public abstract Task<bool> Validate(EvcCommunicationClient evcCommunicationClient, Instrument instrument);
    //    public abstract string GetValue();
    //}
}
