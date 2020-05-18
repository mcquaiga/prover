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
using Prover.Application.Interfaces;
using ReactiveUI;

namespace Prover.UI.Desktop.Views
{
	/// <summary>
	/// Interaction logic for MainMenuUserControl.xaml
	/// </summary>
	[SingleInstanceView]
	public partial class ToolbarMenuView
	{
		public ToolbarMenuView()
		{
			InitializeComponent();

			this.WhenActivated(d =>
			{
				//, value => value.OrderBy(x => x.SortOrder)
				this.OneWayBind(ViewModel, x => x.ToolbarItems, x => x.ToolbarItemsControl.ItemsSource,
							items => items.Where(x => x.ItemType == ToolbarItemType.Module)
										  .OrderBy(i => i.SortOrder)).DisposeWith(d);

				this.OneWayBind(ViewModel, x => x.ToolbarItems, x => x.MainMenuToolbarItemsControl.ItemsSource,
					items => items.Where(x => x.ItemType == ToolbarItemType.MainMenu)
								  .OrderBy(i => i.SortOrder))
					.DisposeWith(d);

				this.OneWayBind(ViewModel, x => x.ActionToolbarItems, x => x.ToolbarActionItemsControl.ItemsSource).DisposeWith(d);
				//ViewModel.ToolbarItems
				//         .OfType<IToolbarActionItem>()
				//		.Observable().BindTo(this, x => x.ToolbarActionItemsControl.ItemsSource).DisposeWith(d);

				this.BindCommand(ViewModel, x => x.NavigateBack, x => x.GoBackButton).DisposeWith(d);
				this.BindCommand(ViewModel, x => x.NavigateHome, x => x.GoHomeButton).DisposeWith(d);
			});
		}
	}

	public class ToolbarItemTemplateSelector : DataTemplateSelector
	{
		/// <inheritdoc />
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			FrameworkElement element = container as FrameworkElement;

			if (element != null && item != null)
			{
				if (item is IToolbarButton button)
				{
					return
							element.FindResource("ToolbarButtonTemplate") as DataTemplate;
				}

				if (item is IToolbarItem barItem)
				{
					return element.FindResource("ToolbarItemTemplate") as DataTemplate;
				}

			}

			return null;

		}
	}
}
