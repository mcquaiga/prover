namespace Prover.Core.VerificationTests.TestActions
{
    using Prover.CommProtocol.Common;
    using Prover.Core.Models.Instruments;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;

    #region Interfaces

    /// <summary>
    /// Defines the <see cref="IInstrumentAction" />
    /// </summary>
    public interface IInstrumentAction
    {
        #region Methods

        ///// <summary>
        ///// The BeforeExecute
        ///// </summary>
        ///// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        ///// <param name="instrument">The instrument<see cref="Instrument"/></param>
        ///// <param name="statusUpdates">The statusUpdates<see cref="Subject{string}"/></param>
        ///// <returns>The <see cref="Task"/></returns>
        //Task BeforeExecute(EvcCommunicationClient commClient, Instrument instrument, Subject<string> statusUpdates = null);

        /// <summary>
        /// The Execute
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="instrument">The instrument<see cref="Instrument"/></param>
        /// <param name="statusUpdates">The statusUpdates<see cref="Subject{string}"/></param>
        /// <returns>The <see cref="Task"/></returns>
        Task Execute(EvcCommunicationClient commClient, Instrument instrument, Subject<string> statusUpdates = null);

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="IPostVerificationAction" />
    /// </summary>
    public interface IPostVerificationAction : IInstrumentAction
    {
    }

    /// <summary>
    /// Defines the <see cref="IPostVolumeTestAction" />
    /// </summary>
    public interface IPostVolumeTestAction : IInstrumentAction
    {
    }

    /// <summary>
    /// Defines the <see cref="IPreVerificationAction" />
    /// </summary>
    public interface IPreVerificationAction : IInstrumentAction
    {
    }

    /// <summary>
    /// Defines the <see cref="IPreVolumeTestAction" />
    /// </summary>
    public interface IPreVolumeTestAction : IInstrumentAction
    {
    }

    #endregion
}
