namespace Prover.CommProtocol.Common.Models.Instrument
{
    using Prover.CommProtocol.Common.Items;
    using Prover.CommProtocol.Common.Models.Instrument.Items;
    using Prover.Core.Shared.Enums;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="EvcDevice" />
    /// </summary>
    internal abstract class EvcDevice : IEvcDevice
    {
        #region Properties

        /// <summary>
        /// Gets or sets the AccessCode
        /// </summary>
        public int AccessCode { get; set; }

        /// <summary>
        /// Gets the CommClient
        /// </summary>
        public EvcCommunicationClient CommClient { get; }

        /// <summary>
        /// Gets the CorrectorType
        /// </summary>
        public EvcCorrectorType CorrectorType { get; }

        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets the ItemDefinitions
        /// </summary>
        public IEnumerable<ItemMetadata> ItemDefinitions { get; }

        /// <summary>
        /// Gets or sets the ItemFilePath
        /// </summary>
        public string ItemFilePath { get; set; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the PressureItems
        /// </summary>
        public IPressureItems PressureItems => throw new NotImplementedException();

        /// <summary>
        /// Gets the SiteInformationItems
        /// </summary>
        public ISiteInformationItems SiteInformationItems => throw new NotImplementedException();

        /// <summary>
        /// Gets the SuperFactorItems
        /// </summary>
        public ISuperFactorItems SuperFactorItems => throw new NotImplementedException();

        /// <summary>
        /// Gets the TemperatureItems
        /// </summary>
        public ITemperatureItems TemperatureItems => throw new NotImplementedException();

        /// <summary>
        /// Gets the VolumeItems
        /// </summary>
        public IVolumeItems VolumeItems => throw new NotImplementedException();

        /// <summary>
        /// Gets the ItemData
        /// </summary>
        protected Dictionary<int, string> ItemData { get; }

        #endregion

        #region Methods

        /// <summary>
        /// The GetAllItems
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public abstract Task GetAllItems();

        /// <summary>
        /// The GetPressureItems
        /// </summary>
        /// <returns>The <see cref="Task{IPressureItems}"/></returns>
        public abstract Task<IPressureItems> GetPressureItems();

        /// <summary>
        /// The GetPressureItems
        /// </summary>
        /// <param name="itemData">The itemData<see cref="Dictionary{string, string}"/></param>
        /// <returns>The <see cref="IPressureItems"/></returns>
        public abstract IPressureItems GetPressureItems(Dictionary<string, string> itemData);

        /// <summary>
        /// The GetTemperatureItems
        /// </summary>
        /// <returns>The <see cref="Task{ITemperatureItems}"/></returns>
        public abstract Task<ITemperatureItems> GetTemperatureItems();

        /// <summary>
        /// The GetTemperatureItems
        /// </summary>
        /// <param name="itemData">The itemData<see cref="Dictionary{string, string}"/></param>
        /// <returns>The <see cref="ITemperatureItems"/></returns>
        public abstract ITemperatureItems GetTemperatureItems(Dictionary<string, string> itemData);

        /// <summary>
        /// The GetVolumeItems
        /// </summary>
        /// <returns>The <see cref="Task{IVolumeItems}"/></returns>
        public abstract Task<IVolumeItems> GetVolumeItems();

        /// <summary>
        /// The GetVolumeItems
        /// </summary>
        /// <param name="itemData">The itemData<see cref="Dictionary{string, string}"/></param>
        /// <returns>The <see cref="IVolumeItems"/></returns>
        public abstract IVolumeItems GetVolumeItems(Dictionary<string, string> itemData);

        #endregion
    }
}
