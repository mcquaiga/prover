﻿namespace Prover.Core.Settings
{
    using Newtonsoft.Json;
    using Prover.Core.Shared.Domain;
    using Prover.Core.Storage;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="SharedSettings" />
    /// </summary>
    public class SharedSettings
    {
        #region Constants

        /// <summary>
        /// Defines the SettingsKey
        /// </summary>
        private const string SettingsKey = "SharedSettings";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the CertificateSettings
        /// </summary>
        public CertificateSettings CertificateSettings { get; set; }

        /// <summary>
        /// Gets or sets the TestSettings
        /// </summary>
        public TestSettings TestSettings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether UpdateAbsolutePressure
        /// </summary>
        public bool UpdateAbsolutePressure { get; set; } = true;

        #endregion

        #region Methods

        /// <summary>
        /// The Create
        /// </summary>
        /// <returns>The <see cref="SharedSettings"/></returns>
        public static SharedSettings Create()
        {
            return new SharedSettings()
            {
                CertificateSettings = new CertificateSettings(),
                TestSettings = TestSettings.CreateDefault()
            };
        }

        /// <summary>
        /// The LoadSharedSettings
        /// </summary>
        /// <param name="keyValueStore">The keyValueStore<see cref="KeyValueStore"/></param>
        /// <returns>The <see cref="SharedSettings"/></returns>
        public static async Task<SharedSettings> LoadSharedSettings(KeyValueStore keyValueStore)
        {
            return await keyValueStore.GetValue<SharedSettings>(SettingsKey) ?? SharedSettings.Create();
        }

        /// <summary>
        /// The SaveSharedSettings
        /// </summary>
        /// <param name="keyValueStore">The keyValueStore<see cref="KeyValueStore"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task SaveSharedSettings(KeyValueStore keyValueStore)
        {
            var kv = new KeyValue() { Id = SettingsKey, Value = JsonConvert.SerializeObject(this) };
            await keyValueStore.Upsert(kv);
        }

        #endregion
    }
}
