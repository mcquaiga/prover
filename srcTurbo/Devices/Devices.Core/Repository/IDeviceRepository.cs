using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Interfaces;

namespace Devices.Core.Repository
{
    public interface IDeviceRepository
    {
        Dictionary<Guid, IDeviceType> Devices { get; }
        Task<T> Find<T>(Func<T, bool> predicate, bool fromCache = true) where T : class, IDeviceType;
        Task<IEnumerable<IDeviceType>> GetAll(bool fromCache = true);
        Task<IEnumerable<T>> GetAll<T>(bool fromCache = true) where T : class, IDeviceType;
        Task<IDeviceType> GetByName(string name, bool fromCache = true);
        IEnumerable<T> FilterCacheByType<T>() where T : class, IDeviceType;
        IEnumerable<IDeviceTypeDataSource<IDeviceType>> FilterDataSourceTypes<TDevice>() where TDevice : IDeviceType;
        Task GetAllDevicesAsync<T>(IEnumerable<IDeviceTypeDataSource<T>> dataSources, bool fromCache = true) where T : class, IDeviceType;
    }
}