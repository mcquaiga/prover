using System;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.Core.Models.Instruments;
using static Prover.Core.VerificationTests.TestActionsManager;

namespace Prover.Core.VerificationTests
{
    public interface ITestActionsManager
    {
        Task RunVerificationCompleteActions(EvcCommunicationClient commClient, Instrument instrument);
        Task RunVolumeTestCompleteActions(EvcCommunicationClient commClient, Instrument instrument);
        Task RunVerificationInitActions(EvcCommunicationClient commClient, Instrument instrument);
        Task RunVolumeTestInitActions(EvcCommunicationClient commClient, Instrument instrument);
        void RegisterAction(TestActionStep actionStep, Func<EvcCommunicationClient, Instrument, Task> testAction);
        void UnregisterActions(TestActionStep actionStep, Func<EvcCommunicationClient, Instrument, Task> testAction);
        Task ExecuteActions(TestActionStep testStep, EvcCommunicationClient commClient, Instrument instrument);
    }
}