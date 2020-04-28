using Microsoft.Extensions.Logging;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace Prover.Application.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject, IDisposable
    {
        protected readonly CompositeDisposable Cleanup = new CompositeDisposable();

        protected readonly ILogger<ViewModelBase> Logger;

        protected ViewModelBase(ILogger<ViewModelBase> logger = null) => Logger = logger ?? ProverLogging.CreateLogger<ViewModelBase>();

        public void Dispose()
        {
            Dispose(true);

            if (!Cleanup.IsDisposed) Cleanup.Dispose();

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
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