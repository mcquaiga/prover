using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Prover.Application.Interfaces
{
    public interface IEventsSubscriber
    {
        void SubscribeToEvents();
    }


  
}