using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Prover.Application.Verifications.CustomActions
{
    public static class ActivatorMixins
    {

    }

    //public class ActivationCoordinator
    //{
    //    private readonly List<IActivatable<IReactiveObject>> _activators;

    //    public ActivationCoordinator(IEnumerable<IActivatable<IReactiveObject>> activators)
    //    {
    //        _activators = activators.ToList();
    //    }

    //    public IActivatable<T> GetActivator<T>() where T : IReactiveObject
    //    {
    //        var my = _activators.FirstOrDefault(x => x.GetType().GenericTypeArguments.Contains(typeof(T)));
    //        return new VerificationActivator<T>();
    //    }

    //    public void RegisterActivator<T>(T activator)
    //    {

    //    }

    //    public void RegisterAction<T>(Action<T> action) where T : IActivatable<T>
    //    {

    //    }
    //}

    public sealed class VerificationActivator<T> : IDisposable
    {
        private IDisposable _activationHandle = Disposable.Empty;
        private readonly List<Func<IEnumerable<IDisposable>>> _blocks;
        private readonly List<Action<T>> _blockFuncs;
        private readonly Subject<T> _activated;
        private readonly Subject<T> _deactivated;
        private int _refCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ReactiveUI.ViewModelActivator" /> class.
        /// </summary>
        public VerificationActivator()
        {
            this._blocks = new List<Func<IEnumerable<IDisposable>>>();
            this._activated = new Subject<T>();
            this._deactivated = new Subject<T>();
            _blockFuncs = new List<Action<T>>();
        }

        /// <summary>
        /// Gets a observable which will tick every time the Activator is activated.
        /// </summary>
        /// <value>The activated.</value>
        public IObservable<T> Activated => this._activated;

        /// <summary>
        /// Gets a observable observable which will tick every time the Activator is deactivated.
        /// </summary>
        /// <value>The deactivated.</value>
        public IObservable<T> Deactivated => this._deactivated;

        /// <summary>
        /// This method is called by the framework when the corresponding View
        /// is activated. Call this method in unit tests to simulate a ViewModel
        /// being activated.
        /// </summary>
        /// <returns>A Disposable that calls Deactivate when disposed.</returns>
        public IDisposable Activate(T item)
        {
            _blockFuncs.ForEach(x => x(item));

            this._activated.OnNext(item);
            return Disposable.Create(() => this.Deactivate(item, false));
        }

        /// <summary>
        /// This method is called by the framework when the corresponding View
        /// is deactivated.
        /// </summary>
        /// <param name="ignoreRefCount">
        /// Force the VM to be deactivated, even
        /// if more than one person called Activate.
        /// </param>
        public void Deactivate(T item, bool ignoreRefCount = false)
        {
            if (!(Interlocked.Decrement(ref this._refCount) == 0 | ignoreRefCount))
                return;
            Interlocked.Exchange<IDisposable>(ref this._activationHandle, Disposable.Empty).Dispose();
            this._deactivated.OnNext(item);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this._activationHandle?.Dispose();
            this._activated?.Dispose();
            this._deactivated?.Dispose();
        }

        public void AddActivationBlock(Action<T> block)
        {
            _blockFuncs.Add(block);
        }

        /// <summary>
        /// Adds a action blocks to the list of registered blocks. These will called
        /// on activation, then disposed on deactivation.
        /// </summary>
        /// <param name="block">The block to add.</param>
        internal void AddActivationBlock(Func<IEnumerable<IDisposable>> block)
        {
            this._blocks.Add(block);
        }
    }
}
