namespace Prover.Core.Settings
{
    using Prover.Core.IO;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="LocalSettings" />
    /// </summary>
    public class LocalSettings
    {
        #region Properties

        public bool AutoSave { get; set; }

        /// <summary>
        /// Gets or sets the InstrumentBaudRate
        /// </summary>
        public int InstrumentBaudRate { get; set; }

        /// <summary>
        /// Gets or sets the InstrumentCommPort
        /// </summary>
        public string InstrumentCommPort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether InstrumentUseIrDaPort
        /// </summary>
        public bool InstrumentUseIrDaPort { get; set; }

        /// <summary>
        /// Gets or sets the LastClientSelected
        /// </summary>
        public string LastClientSelected { get; set; }

        /// <summary>
        /// Gets or sets the LastInstrumentTypeUsed
        /// </summary>
        public string LastInstrumentTypeUsed { get; set; }

        /// <summary>
        /// Gets or sets the TachCommPort
        /// </summary>
        public string TachCommPort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether TachIsNotUsed
        /// </summary>
        public bool TachIsNotUsed { get; set; }

        /// <summary>
        /// Gets or sets the WindowHeight
        /// </summary>
        public double WindowHeight { get; set; } = 800;

        /// <summary>
        /// Gets or sets the WindowState
        /// </summary>
        public string WindowState { get; set; } = "Normal";

        /// <summary>
        /// Gets or sets the WindowWidth
        /// </summary>
        public double WindowWidth { get; set; } = 800;

        #endregion

        #region Methods

        /// <summary>
        /// The LoadLocalSettings
        /// </summary>
        /// <param name="filePath">The filePath<see cref="string"/></param>
        /// <returns>The <see cref="Task{LocalSettings}"/></returns>
        public static async Task<LocalSettings> LoadLocalSettings(string filePath)
        {
            var fileSystem = new FileSystem();
            
            return await new SettingsReader(fileSystem).Read(filePath).ConfigureAwait(false);
        }

        /// <summary>
        /// The SaveLocalSettings
        /// </summary>
        /// <param name="filePath">The filePath<see cref="string"/></param>
        public async Task SaveLocalSettings(string filePath)
        {
            var fileSystem = new FileSystem();

            if (!fileSystem.DirectoryExists(Path.GetDirectoryName(filePath)))
                fileSystem.CreateDirectory(Path.GetDirectoryName(filePath));

            await new SettingsWriter(fileSystem).Write(filePath, this).ConfigureAwait(false);
        }

        /// <summary>
        /// The SaveLocalSettingsAsync
        /// </summary>
        /// <param name="filePath">The filePath<see cref="string"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task SaveLocalSettingsAsync(string filePath)
        {
            await SaveLocalSettings(filePath);
        }

        #endregion
    }
}
