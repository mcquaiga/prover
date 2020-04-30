using System.Threading.Tasks;

namespace Prover.Application.Settings
{
    /// <summary>
    /// Defines the <see cref="ISettingsService" />
    /// </summary>
    public interface ISettingsService
    {
        #region Properties

        /// <summary>
        /// Gets the CertificateSettings
        /// </summary>
        CertificateSettings CertificateSettings { get; }

        /// <summary>
        /// Gets the Local
        /// </summary>
        LocalSettings Local { get; }

        /// <summary>
        /// Gets the Shared
        /// </summary>
        SharedSettings Shared { get; }

        /// <summary>
        /// Gets the TestSettings
        /// </summary>
        TestSettings TestSettings { get; }

        bool HasInitialized { get; }

        #endregion

        #region Methods

        /// <summary>
        /// The RefreshSettings
        /// </summary>
        /// <returns>The <see cref="System.Threading.Tasks.Task{TResult}"/></returns>
        Task RefreshSettings();

        /// <summary>
        /// The SaveSettings
        /// </summary>
        /// <returns>The <see cref="System.Threading.Tasks.Task{TResult}"/></returns>
        Task SaveSettings();

        #endregion
    }
}