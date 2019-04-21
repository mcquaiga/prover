using System;
using System.Threading.Tasks;

namespace Module.EvcVerification.VerificationTests.TestActions
{
    public interface ITestActionsManager
    {
        Task ExecuteValidations(VerificationStep verificationStep, EvcCommunicationClient commClient, Instrument instrument);
        void RegisterAction(VerificationStep verificationStep, Func<EvcCommunicationClient, Instrument, Task> testAction);
        void UnregisterActions(VerificationStep verificationStep, Func<EvcCommunicationClient, Instrument, Task> testAction);
    }
}