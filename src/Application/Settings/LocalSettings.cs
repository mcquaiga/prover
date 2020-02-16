using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Shared.Domain;
using Shared.Interfaces;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Application.Settings
{
    /// <summary>
    /// Defines the <see cref="LocalSettings" />
    /// </summary>
    public class LocalSettings : IKeyValueEntity
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

        public LocalSettings()
        {
        }

        public string Key => "LocalSettings";
    }

}
