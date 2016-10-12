using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Instruments;

namespace Prover.Core.ExternalIntegrations
{
    public interface IVerifier
    {
        Task<object> Verify(EvcCommunicationClient evcCommunicationClient, Instrument instrument);
    }

    public interface IUpdater
    {
        Task<object> Update(EvcCommunicationClient evcCommunicationClient, Instrument instrument);
        Task<object> GetNewValue();
    }

    public abstract class VerifierUpdaterBase
    {
        protected VerifierUpdaterBase()
        {

        }

        public async Task VerifyUpdate(EvcCommunicationClient commClient, Instrument instrument)
        {
            await Verify(commClient, instrument);
            await Update(commClient, instrument);
        }

        protected abstract Task<object> Verify(EvcCommunicationClient commClient, Instrument instrument);
        protected abstract Task<object> Update(EvcCommunicationClient commClient, Instrument instrument);
    }
}