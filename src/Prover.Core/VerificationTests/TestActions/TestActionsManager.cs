namespace Prover.Core.VerificationTests
{
    using Prover.CommProtocol.Common;
    using Prover.Core.Models.Instruments;
    using Prover.Core.VerificationTests.TestActions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="TestActionsManager" />
    /// </summary>
    public class TestActionsManager : ITestActionsManager
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestActionsManager"/> class.
        /// </summary>
        /// <param name="preVolumeTestActions">The preVolumeTestActions<see cref="IEnumerable{IPreVolumeTestAction}"/></param>
        /// <param name="postVolumeTestActions">The postVolumeTestActions<see cref="IEnumerable{IPostVolumeTestAction}"/></param>
        /// <param name="preVerificationActions">The preVerificationActions<see cref="IEnumerable{IPreVerificationAction}"/></param>
        /// <param name="postVerificationActions">The postVerificationActions<see cref="IEnumerable{IPostVerificationAction}"/></param>
        public TestActionsManager(IEnumerable<IEvcDeviceValidationAction> verificationValidationActions)
        {
            VerificationValidationActions = verificationValidationActions;
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<IEvcDeviceValidationAction> VerificationValidationActions { get; }

        private List<Tuple<VerificationStep, Func<EvcCommunicationClient, Instrument, Task>>> _dynamicActions { get; }
            = new List<Tuple<VerificationStep, Func<EvcCommunicationClient, Instrument, Task>>>();

        #endregion Properties

        public async Task ExecuteValidations(VerificationStep verificationStep, EvcCommunicationClient commClient, Instrument instrument)
        {
            if (!commClient.IsConnected)
            {
                await commClient.Connect(new CancellationToken());
            }

            foreach (IEvcDeviceValidationAction testAction in VerificationValidationActions.Where(vv => vv.VerificationStep == verificationStep))
            {
                await testAction.Execute(commClient, instrument).ConfigureAwait(false);
            }

            foreach (Tuple<VerificationStep, Func<EvcCommunicationClient, Instrument, Task>> a in _dynamicActions.Where(a => a.Item1 == verificationStep))
            {
                await a.Item2.Invoke(commClient, instrument);
            }
        }

        public void RegisterAction(VerificationStep verificationStep, Func<EvcCommunicationClient, Instrument, Task> testAction)
        {
            _dynamicActions.Add(new Tuple<VerificationStep, Func<EvcCommunicationClient, Instrument, Task>>(verificationStep, testAction));
        }

        public void UnregisterActions(VerificationStep verificationStep, Func<EvcCommunicationClient, Instrument, Task> testAction)
        {
            _dynamicActions.RemoveAll(x => x.Item1 == verificationStep && x.Item2 == testAction);
        }
    }
}