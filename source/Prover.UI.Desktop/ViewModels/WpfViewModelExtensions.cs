using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using Prover.UI.Desktop.Controls;

namespace Prover.UI.Desktop.ViewModels
{
	public static class WpfViewModelExtensions
	{
		public static IDisposable AddToolbarItem(this IToolbarManager toolbar, ICommand command, PackIconKind iconKind)
		{
			return toolbar.AddToolbarItem(new ToolbarActionItem(iconKind.ToString(), command));
		}

		public static IDisposable AddToolbarItem(this IToolbarManager toolbar, IEnumerable<IToolbarActionItem> toolbarItems)
		{
			return new CompositeDisposable(toolbarItems.Select(i => toolbar.AddToolbarItem(i)));
		}
	}
}