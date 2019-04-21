using Devices.Core.Interfaces;
using Devices.Core.Items;
using System.Collections.Generic;

namespace Devices.Honeywell.Core
{
    /// <summary>
    /// Defines the <see cref="HoneywellEvcType"/>
    /// </summary>
    public class HoneywellEvcType : IEvcDeviceType
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