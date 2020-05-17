using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using Prover.Application.Extensions;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using Prover.UI.Desktop.Controls;
using ReactiveUI;

namespace Prover.UI.Desktop.ViewModels
{
	public class ToolbarManager : ViewModelBase, IToolbarManager
	{
		/// <inheritdoc />
		private readonly SourceList<IToolbarActionItem> _toolbarItems = new SourceList<IToolbarActionItem>();

		public ToolbarManager(IScreenManager screenManager, IEnumerable<IModuleToolbarItem> moduleToolbarItems)
		{
			var items = _toolbarItems.Connect()
									 .ObserveOn(RxApp.MainThreadScheduler)
									 .Bind(out var toolbarItems)
									 .DisposeMany()
									 .Subscribe();

			ActionToolbarItems = toolbarItems;


			//Sort(x => x.SortOrder)
			//.AsObservableList();

			//items.Filter(i => i.IsTypeOrInheritsOf(typeof(IToolbarActionItem)))
			//	 .Transform((i => i as IToolbarActionItem))
			//	 .ObserveOn(RxApp.MainThreadScheduler)
			//	 .Bind(out var actionItems)
			//	 .DisposeMany()
			//	 .Subscribe();//.ToListObservable();// ToolbarItems(i => i.IsTypeOf(typeof(IToolbarActionItem))).ToListObservable();
			//ActionToolbarItems = toolbarItems; //.ToObservable().Where(x => x.IsTypeOrInheritsOf(typeof(IToolbarActionItem)));


			NavigateForward = ReactiveCommand.CreateFromTask<IRoutableViewModel, IRoutableViewModel>(screenManager.ChangeView);
			NavigateBack = ReactiveCommand.CreateFromTask(screenManager.GoBack, screenManager.Router.NavigateBack.CanExecute);
			NavigateHome = ReactiveCommand.CreateFromTask(async () => { await screenManager.GoHome(); });
			ToolbarItems = moduleToolbarItems.ToList();
			//items.DisposeWith(Cleanup);
		}

		public ReadOnlyObservableCollection<IToolbarActionItem> ActionToolbarItems { get; set; }

		public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> NavigateForward { get; }
		public ReactiveCommand<Unit, Unit> NavigateBack { get; }
		public ReactiveCommand<Unit, Unit> NavigateHome { get; }

		public ICollection<IModuleToolbarItem> ToolbarItems { get; } //=> _toolbarItems.Connect().AsObservableList();

		public IDisposable AddToolbarItem(IToolbarActionItem item)
		{
			_toolbarItems.Add(item);
			return Disposable.Create(() => { _toolbarItems.Remove(item); });
		}

		/*
		 * 	if (viewModel is IHaveToolbarItems barItems)
			{
				_toolbarRemover.Disposable = Disposable.Empty;

				var disposables = barItems.ToolbarActionItems.Select(AddToolbarItem).ToList();

				_toolbarRemover.Disposable = new CompositeDisposable(disposables.ToList());
				//.Prepend(viewModel as IDisposable));
			}
		 */
	}
}