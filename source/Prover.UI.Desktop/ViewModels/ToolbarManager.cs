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
		private readonly IEnumerable<IToolbarItem> _toolbarItems;

		/// <inheritdoc />
		private readonly SourceList<IToolbarButton> _actionItems = new SourceList<IToolbarButton>();

		public ToolbarManager(IScreenManager screenManager, IEnumerable<IToolbarItem> toolbarItems)
		{
			_toolbarItems = toolbarItems;
			//AppMainMenus = toolbarItems
			//ToolbarItems = toolbarItems.Where(x => x.ItemType == ToolbarItemType.Module).ToList();


			_actionItems.Connect()
						 .ObserveOn(RxApp.MainThreadScheduler)
						 .Bind(out var actionItems)
						 .DisposeMany()
						 .Subscribe()
						 .DisposeWith(Cleanup);
			ActionToolbarItems = actionItems;


			NavigateForward = ReactiveCommand.CreateFromTask<IRoutableViewModel, IRoutableViewModel>(screenManager.ChangeView);
			NavigateBack = ReactiveCommand.CreateFromTask(screenManager.GoBack, screenManager.Router.NavigateBack.CanExecute);
			NavigateHome = ReactiveCommand.CreateFromTask(async () => { await screenManager.GoHome(); });


			NavigateForward.DisposeWith(Cleanup);
			NavigateBack.DisposeWith(Cleanup);
			NavigateHome.DisposeWith(Cleanup);
		}

		public ReadOnlyObservableCollection<IToolbarButton> ActionToolbarItems { get; set; }

		public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> NavigateForward { get; }
		public ReactiveCommand<Unit, Unit> NavigateBack { get; }
		public ReactiveCommand<Unit, Unit> NavigateHome { get; }

		//public ICollection<IToolbarItem> AppMainMenus => _toolbarItems.Where(x => x.ItemType == ToolbarItemType.MainMenu).OrderBy(x => x.SortOrder).ToList();
		public ICollection<IToolbarItem> ToolbarItems => _toolbarItems.ToList();

		public IDisposable AddToolbarItem(IToolbarButton item)
		{
			_actionItems.Add(item);
			return Disposable.Create(() => { _actionItems.Remove(item); });
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