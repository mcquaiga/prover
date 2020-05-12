using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DynamicData.Binding;
using Prover.Application.Interactions;
using ReactiveUI;

namespace Prover.UI.Desktop.Views
{
	/// <summary>
	/// Interaction logic for MainMenuUserControl.xaml
	/// </summary>
	[SingleInstanceView]
	public partial class MainMenuUserControl
	{
		public MainMenuUserControl()
		{
			InitializeComponent();

			this.WhenActivated(d =>
			{
				this.OneWayBind(ViewModel, x => x.ToolbarItems, x => x.ToolbarItemsControl.ItemsSource, value => value.OrderBy(x => x.SortOrder)).DisposeWith(d);
				ViewModel.ToolbarItems
						 .ToObservableChangeSet()
						 .OfType<IToolbarActionItem>().ToListObservable().BindTo(this, x => x.ToolbarActionItemsControl.ItemsSource).DisposeWith(d);

				this.BindCommand(ViewModel, x => x.NavigateBack, x => x.GoBackButton).DisposeWith(d);
				this.BindCommand(ViewModel, x => x.NavigateHome, x => x.GoHomeButton).DisposeWith(d);
			});
		}
	}
}
