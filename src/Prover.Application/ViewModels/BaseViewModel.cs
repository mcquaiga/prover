using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;

namespace Prover.Application.ViewModels
{
    public static class ViewModelRegisteration
    {
        public static ConcurrentDictionary<Guid, ViewModelBase> ViewModels { get; } = new ConcurrentDictionary<Guid, ViewModelBase>();

        public static void Register(this ViewModelBase viewModel, Guid id)
        {
            if (!ViewModels.TryAdd(id, viewModel))
            {
                Debug.WriteLine($"ERROR - Could not add {viewModel} - Id: {id}");
            }
            //Debug.WriteLine($"   -> ViewModels {ViewModels.Count}");
        }
        
        public static ViewModelBase Unregister(this ViewModelBase viewModel, Guid id)
        {
            if (ViewModels.TryRemove(id, out var vm))
            {
                //Debug.WriteLine($"   -> ViewModels {ViewModels.Count}");
                return vm;
            }

            Debug.WriteLine($"ViewModel Id: {id} was not registered");
            return (ViewModelBase) null;
        }
    }

    public abstract class ViewModelBase : ReactiveObject,  IDisposable, IActivatableViewModel
    {
        private readonly Guid _id = Guid.NewGuid();

        protected CompositeDisposable Cleanup { get; } = new CompositeDisposable();

        protected ViewModelBase()
        {
            Activator.Activated
                //.LogDebug($"Activated - {this} - {_id}")
                .Do(x => this.Register(_id))
                .Subscribe();

            Activator.Deactivated
                //.LogDebug($"Deactivated - {this} - {_id}")
                .Do(x => this.Unregister(_id))
                .Subscribe();
        }

        public void Dispose()
        {
            Debug.WriteLine($"Disposing - {this}");
            Disposing();
            Cleanup.Dispose();
        }

        protected virtual void Disposing()
        {

        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }

    public abstract class ViewModelWithIdBase : ViewModelBase, IActivatableViewModel
    {
        protected ViewModelWithIdBase() : this(Guid.NewGuid())
        {
        }

        protected ViewModelWithIdBase(Guid id)
        {
            Id = id;
        }

       

        public Guid Id { get; protected set; }

      
  
    }
}