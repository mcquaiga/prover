using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using DynamicData;
using Prover.Application.Interactions;
using ReactiveUI;

namespace Prover.Application.Interfaces {

	public interface IToolbarManager {
		ICollection<IToolbarItem> ToolbarItems { get; }
		ReadOnlyObservableCollection<IToolbarButton> ActionToolbarItems { get; set; }
		IDisposable AddToolbarItem(IToolbarButton item);
	}

	public interface IScreenManager : IScreen {
		Task<TViewModel> ChangeView<TViewModel>(TViewModel viewModel) where TViewModel : IRoutableViewModel;
		Task<TViewModel> ChangeView<TViewModel>(params object[] parameters) where TViewModel : IRoutableViewModel;

		IDialogServiceManager DialogManager { get; }

		Task GoHome(IRoutableViewModel viewModel = null);
		Task GoBack();

		//ReadOnlyObservableCollection<IToolbarActionItem> ToolbarItems { get; }
		//IDisposable AddToolbarItem(IToolbarActionItem item);
	}
}