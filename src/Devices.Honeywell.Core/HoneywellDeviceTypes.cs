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

        public static IEvcDeviceType Ec350 => GetByName("EC-350");

        public static IEvcDeviceType MiniAt => GetByName("Mini-AT");

        public static IEvcDeviceType MiniMax => GetByName("Mini-Max");

        public static IEvcDeviceType Tci => GetByName("Tci");

        public static IEvcDeviceType TibBoard => GetByName("TibBoard");

        public static IEvcDeviceType Toc => GetByName("Toc");

        #endregion

        #region Methods

        public static IEnumerable<IEvcDeviceType> GetAll(bool showHidden = false)
        {
            var allTask = _typesInstance.LoadDevicesAsync();
            return allTask.GetAwaiter().GetResult()
                .Where(evc => showHidden || !evc.IsHidden)
                .OrderBy(i => i.Name);
        }

        public static IEvcDeviceType GetById(int id)
        {
            var all = GetAll(true);

            return all.FirstOrDefault(i => i.Id == id);
        }

        public static IEvcDeviceType GetByName(string name)
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