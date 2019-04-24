using Devices.Core.Interfaces;
using Devices.Core.Items;
using System.Collections.Generic;

namespace Devices.Honeywell.Core
{
    public interface IHoneywellEvcType : IEvcDeviceType
    {
        #region Properties

        int AccessCode { get; set; }

        int Id { get; set; }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="HoneywellEvcType"/>
    /// </summary>
    public class HoneywellEvcType : IHoneywellEvcType
    {
        #region Properties

        public int AccessCode { get; set; }

        public bool? CanUseIrDaPort { get; set; }

        public IEnumerable<ItemMetadata> Definitions { get; set; }

        public int Id { get; set; }

        public bool IsHidden { get; set; } = false;

        public string ItemFilePath { get; set; }

        public int? MaxBaudRate { get; set; }

        public string Name { get; set; }

        #endregion
    }
}