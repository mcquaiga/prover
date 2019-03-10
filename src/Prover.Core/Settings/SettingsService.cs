namespace Prover.Core.Settings
{
    using Autofac;
    using Caliburn.Micro;
    using Prover.Core.Events;
    using Prover.Core.Shared.Components;
    using Prover.Core.Storage;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    #region Interfaces

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

        #endregion

        #region Methods

        /// <summary>
        /// The RefreshSettings
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        Task RefreshSettings();

        /// <summary>
        /// The SaveSettings
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        Task SaveSettings();

        #endregion
    }

    #endregion

    /// <summary>
    /// Defines the <see cref="SettingsService" />
    /// </summary>
    public class SettingsService : ISettingsService, IStartable
    {
        #region Constants

        /// <summary>
        /// Defines the SettingsFileName
        /// </summary>
        private const string SettingsFileName = "settings.conf";

        #endregion

        #region Fields

        /// <summary>
        /// Defines the AppDirectory
        /// </summary>
        private static readonly string AppDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EvcProver");

        /// <summary>
        /// Defines the SettingsPath
        /// </summary>
        private static readonly string SettingsPath = Path.Combine(AppDirectory, SettingsFileName);

        /// <summary>
        /// Defines the _keyValueStore
        /// </summary>
        private readonly KeyValueStore _keyValueStore;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsService"/> class.
        /// </summary>
        /// <param name="keyValueStore">The keyValueStore<see cref="KeyValueStore"/></param>
        /// <param name="eventAggregator">The eventAggregator<see cref="IEventAggregator"/></param>
        public SettingsService(KeyValueStore keyValueStore, IEventAggregator eventAggregator)
        {
            _keyValueStore = keyValueStore;
            EventAggregator = eventAggregator;
            HasInitialized = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the CertificateSettings
        /// </summary>
        public CertificateSettings CertificateSettings => Shared.CertificateSettings;

        /// <summary>
        /// Gets the EventAggregator
        /// </summary>
        public IEventAggregator EventAggregator { get; }

        /// <summary>
        /// Gets a value indicating whether HasInitialized
        /// </summary>
        public bool HasInitialized { get; private set; }

        /// <summary>
        /// Gets the Local
        /// </summary>
        public LocalSettings Local { get; private set; }

        /// <summary>
        /// Gets the Shared
        /// </summary>
        public SharedSettings Shared { get; private set; }

        /// <summary>
        /// Gets the TestSettings
        /// </summary>
        public TestSettings TestSettings => Shared.TestSettings;

        #endregion

        #region Methods

        /// <summary>
        /// The RefreshSettings
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public async Task RefreshSettings()
        {
            HasInitialized = false;
            Local = await LocalSettings.LoadLocalSettings(SettingsPath);
            Shared = await SharedSettings.LoadSharedSettings(_keyValueStore);
            HasInitialized = true;
        }

        /// <summary>
        /// The SaveSettings
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public async Task SaveSettings()
        {
            await Shared.SaveSharedSettings(_keyValueStore);
            await Local.SaveLocalSettingsAsync(SettingsPath);

            await EventAggregator.PublishOnUIThreadAsync(new SettingsChangeEvent());
        }

        /// <summary>
        /// The Start
        /// </summary>
        public void Start()
        {
            AsyncUtil.RunSync(RefreshSettings);
        }

        #endregion
    }
}
