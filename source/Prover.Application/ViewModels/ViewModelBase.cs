using System;
using System.Reactive.Disposables;
using System.Threading;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace Prover.Application.ViewModels
{
	public abstract class ViewModelBase : ReactiveObject, IActivatableViewModel, IDisposable
	{
		private readonly Lazy<ViewModelActivator> _activatorLazy = new Lazy<ViewModelActivator>(() => new ViewModelActivator());
		private readonly ViewModelActivator _activator = new ViewModelActivator();

		protected readonly ILogger<ViewModelBase> Logger;
		protected CompositeDisposable Cleanup = new CompositeDisposable();
		protected CompositeDisposable DeactivateDisposer = new CompositeDisposable();

		protected ViewModelBase(ILogger<ViewModelBase> logger = null)
		{
			Logger = logger ?? ProverLogging.CreateLogger<ViewModelBase>();

			(this as IActivatableViewModel)?
					.WhenActivated(disposables =>
					{
						DeactivateDisposer = new CompositeDisposable();

						HandleActivation(disposables);

						Disposable
								.Create(() => this.HandleDeactivation())
								.DisposeWith(disposables);

						DeactivateDisposer
								.DisposeWith(disposables);
					});
		}

		protected virtual void HandleDeactivation() { }

		protected virtual void HandleActivation(CompositeDisposable cleanup) { }

		/// <inheritdoc />
		public CancellationToken OnChanging { get; set; }

		/// <inheritdoc />
		public ViewModelActivator Activator => _activator;

		/// <inheritdoc />
		public virtual bool CanNavigateAway() => true;

		public void Dispose()
		{
			if (Cleanup != null && !Cleanup.IsDisposed)
				Cleanup.Dispose();

			Dispose(true);

			//GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
		}

		protected virtual void SetupActivator(ViewModelBase viewModel)
		{
			//if (viewModel is IActivatableViewModel activatable)


		}
	}
}