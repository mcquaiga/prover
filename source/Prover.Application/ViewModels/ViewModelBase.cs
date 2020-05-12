using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;
using Prover.Application.Interactions;
using ReactiveUI;

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

			if (!Cleanup.IsDisposed)
				Cleanup.Dispose();

			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
		}

		private readonly ICollection<IToolbarActionItem> _toolbarActionItems = new List<IToolbarActionItem>();
		/// <inheritdoc />
		public IEnumerable<IToolbarActionItem> ToolbarActionItems => _toolbarActionItems;

		public void AddToolbarItem(IToolbarActionItem toolbarItem)
		{
			_toolbarActionItems.Add(toolbarItem);
		}
	}
}