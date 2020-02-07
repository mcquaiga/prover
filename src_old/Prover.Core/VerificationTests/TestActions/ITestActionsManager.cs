using System;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests.TestActions;

namespace Prover.Core.VerificationTests
{
    public interface ITestActionsManager
    {
        Task ExecuteValidations(VerificationStep verificationStep, EvcCommunicationClient commClient, Instrument instrument);
        void RegisterAction(VerificationStep verificationStep, Func<EvcCommunicationClient, Instrument, Task> testAction);
        void UnregisterActions(VerificationStep verificationStep, Func<EvcCommunicationClient, Instrument, Task> testAction);
    }
}