using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Domain;
using Shared.Interfaces;

namespace Application.Settings
{
    /// <summary>
    /// Defines the <see cref="SharedSettings" />
    /// </summary>
    public class SharedSettings : IKeyValueEntity
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

       

        #endregion

        public string Key => "SharedSettings";
    }
}
