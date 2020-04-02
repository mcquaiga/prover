using System;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ReactiveUI;

namespace Prover.Application.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject, IDisposable
    {
        protected readonly CompositeDisposable Cleanup = new CompositeDisposable();

        protected readonly ILogger Logger;

        protected ViewModelBase(ILogger logger = null) => Logger = logger ?? NullLogger.Instance;


        public void Dispose()
        {
            Logger.LogDebug($"Disposing - {this}");
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

        public Guid Id { get; protected set; }
    }
}