using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.Application.Services.LiveReadCorrections;

namespace Prover.Application.Interfaces
{
	public interface ILiveReadHandler
	{
		ICollection<ItemLiveReadStatus> LiveReadItems { get; set; }
		IObservable<ItemLiveReadStatus> LiveReadUpdates { get; }
		Task<IObservable<ItemLiveReadStatus>> Start();
		Task Stop();
	}
}