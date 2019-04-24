using Devices.Core.Interfaces;
using Prover.CommProtocol.MiHoneywell.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Devices.Honeywell.Core
{
    public static class HoneywellDeviceTypes
    {
        #region Properties

        public static IHoneywellEvcType Ec350 => GetByName("EC-350");

        public static IHoneywellEvcType MiniAt => GetByName("Mini-AT");

        public static IHoneywellEvcType MiniMax => GetByName("Mini-Max");

        public static IHoneywellEvcType Tci => GetByName("Tci");

        public static IHoneywellEvcType TibBoard => GetByName("TibBoard");

        public static IHoneywellEvcType Toc => GetByName("Toc");

        #endregion

        #region Methods

        public static IEnumerable<IHoneywellEvcType> GetAll(bool showHidden = false)
        {
            var allTask = _typesInstance.LoadDevicesAsync();
            return allTask.GetAwaiter().GetResult()
                .Where(evc => showHidden || !evc.IsHidden)
                .OrderBy(i => i.Name);
        }

        public static IHoneywellEvcType GetById(int id)
        {
            var all = GetAll(true);

            return all.FirstOrDefault(i => i.Id == id);
        }

        public static IHoneywellEvcType GetByName(string name)
        {
            var all = GetAll(true);

            return all.FirstOrDefault(i => string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region Fields

        private static Lazy<DeviceTypeLoader> _devices = new Lazy<DeviceTypeLoader>(() => new DeviceTypeLoader());

        #endregion

        private static DeviceTypeLoader _typesInstance => _devices.Value;
    }
}