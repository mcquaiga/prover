using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devices.Core.Interfaces
{
    public interface ILoadDeviceTypes<TEvcType> where TEvcType : IEvcDeviceType
    {
        #region Methods

        IReadOnlyCollection<TEvcType> EvcDeviceTypes { get; }

        Task<IEnumerable<TEvcType>> LoadDevicesAsync();

        #endregion
    }
}