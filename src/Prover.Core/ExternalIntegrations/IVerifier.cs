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
    }
}