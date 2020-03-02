using System;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ReactiveUI;

namespace Prover.Application.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject, IDisposable, IActivatableViewModel
    {
        private readonly Guid _id = Guid.NewGuid();
        private readonly ILogger Logger;

        protected ViewModelBase(ILogger logger = null) => Logger = logger ?? NullLogger.Instance;

        protected CompositeDisposable Cleanup { get; } = new CompositeDisposable();

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

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

    public abstract class ViewModelWithIdBase : ViewModelBase, IActivatableViewModel
    {
        protected ViewModelWithIdBase() : this(Guid.NewGuid())
        {
        }

        protected ViewModelWithIdBase(Guid id) => Id = id;


        public Guid Id { get; protected set; }
    }
}