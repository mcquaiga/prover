namespace Module.EvcVerification.VerificationTests.TestActions
{
    using System.Reactive.Subjects;
    using System.Threading;
    using System.Threading.Tasks;

    public enum VerificationStep
    {
        PreVerification,
        PostVerification,
        PreVolumeVerification,
        PostVolumeVerification
    }

    #region Interfaces

    /// <summary>
    /// Defines the <see cref="IEvcDeviceValidationAction" />
    /// </summary>
    public interface IEvcDeviceValidationAction
    {
        VerificationStep VerificationStep { get; }

        #region Methods

        /// <summary>
        /// The Execute
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="instrument">The instrument<see cref="Instrument"/></param>
        /// <param name="statusUpdates">The statusUpdates<see cref="Subject{string}"/></param>
        /// <returns>The <see cref="Task"/></returns>
        Task Execute(EvcCommunicationClient commClient, Instrument instrument,
            CancellationToken ct = new CancellationToken(), Subject<string> statusUpdates = null);

        #endregion Methods
    }

    #endregion Interfaces
}