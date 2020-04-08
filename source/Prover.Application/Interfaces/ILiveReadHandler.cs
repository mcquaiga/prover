using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Devices.Core.Items;
using Prover.Application.Services.LiveReadCorrections;

namespace Prover.Application.Interfaces
{
    public interface ILiveReadHandler
    {
        Task<IObservable<ItemLiveReadStatus>> Start();
        Task Stop();
    }
}