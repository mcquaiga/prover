using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Devices.Core.Items;

namespace Prover.Application.Interfaces
{
    public interface ILiveReadHandler
    {
        Task<IObservable<ItemValue>> Start(IDeviceSessionManager deviceSession);
        IObservable<ItemValue> LiveReadUpdates { get; set; }
    }
}