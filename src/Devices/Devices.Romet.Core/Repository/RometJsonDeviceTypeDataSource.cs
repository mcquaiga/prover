using System;
using System.Collections.Generic;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using Devices.Core.Repository.JsonConverters;

namespace Devices.Romet.Core.Repository
{
    public static class RometDeviceRepository
    {
        public static string Adem = "Adem";

        private static readonly Lazy<DeviceRepository> _lazy = new Lazy<DeviceRepository>(Factory);

        #region Public Properties

        public static DeviceType GetAdem => Devices.GetByName(Adem);

        public static IDeviceTypeDataSource<DeviceType> DataSource => RometJsonDeviceTypeDataSource.Instance;

        public static DeviceRepository Devices => _lazy.Value;

        #endregion

        #region Private

        private static DeviceRepository Factory()
        {
            return new DeviceRepository().RegisterDataSource(DataSource);
        }

        #endregion
    }

    public class RometJsonDeviceTypeDataSource : JsonDeviceTypeDataSource<RometDeviceType>
    {
        private static readonly string _directory = $"{AppDomain.CurrentDomain.BaseDirectory}\\RometDeviceTypes";

        private static readonly Lazy<IDeviceTypeDataSource<RometDeviceType>> _lazy
            = new Lazy<IDeviceTypeDataSource<RometDeviceType>>(Factory);

        public RometJsonDeviceTypeDataSource(IStreamReader streamReader) : base(streamReader)
        {
        }

        private RometJsonDeviceTypeDataSource()
            : base(new FileStreamReader(_directory))
        {
        }

        #region Public Properties

        public static IDeviceTypeDataSource<RometDeviceType> Instance { get; } = _lazy.Value;

        #endregion

        #region Protected

        protected override JsonDeviceConverter<RometDeviceType> DeviceConverter(IEnumerable<ItemMetadata> items,
            JsonItemsConverter itemsConverter)
        {
            return new RometJsonDeviceConverter(items, itemsConverter);
        }

        #endregion

        #region Private

        private static IDeviceTypeDataSource<RometDeviceType> Factory()
        {
            return new RometJsonDeviceTypeDataSource();
        }

        #endregion
    }
}