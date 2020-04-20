using System;
using System.Threading.Tasks;
using Prover.Application.Services.LiveReadCorrections;

namespace Prover.Application.Interfaces
{
    public interface ILiveReadHandler
    {
        Task<IObservable<ItemLiveReadStatus>> Start();
        Task Stop();
    }
}