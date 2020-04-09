using System;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interfaces;
using ReactiveUI;

namespace Prover.Application.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject, IDisposable
    {
        protected readonly CompositeDisposable Cleanup = new CompositeDisposable();

        protected readonly ILogger<ViewModelBase> Logger;

        protected ViewModelBase(ILogger<ViewModelBase> logger = null) => Logger = logger ?? NullLogger<ViewModelBase>.Instance;
        
        public void Dispose()
        {
            Logger.LogTrace($"Disposing - {this}");
            Disposing();
            Cleanup.Dispose();
        }

        protected virtual void Disposing()
        {
        }
    }

    public abstract class ViewModelWithIdBase : ViewModelBase
    {
        protected ViewModelWithIdBase() : this(Guid.NewGuid())
        {
        }

        protected ViewModelWithIdBase(Guid id) => Id = id;

        public Guid Id { get; set; }
    }
}