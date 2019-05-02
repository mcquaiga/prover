using System.Collections.Generic;
using System.IO;

namespace Devices.Core.Interfaces
{
    public interface IStreamReader
    {
        #region Methods

        IEnumerable<StreamReader> GetDeviceTypeReaders();

        StreamReader GetItemDefinitionsReader(string name);

        StreamReader GetItemsReader();

        #endregion
    }
}