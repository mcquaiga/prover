using System.Collections.Generic;
using System.Linq;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using Prover.UI.Desktop.Controls;
using ReactiveUI.Fody.Helpers;

namespace Prover.Modules.DevTools
{
	public class DevToolbarMenu : ViewModelBase, IToolbarItem
	{
		public DevToolbarMenu(IEnumerable<IDevToolsMenuItem> devMenuItems = null) => MenuItems = devMenuItems.ToList();

		[Reactive] public ICollection<IDevToolsMenuItem> MenuItems { get; set; }

		/// <inheritdoc />
		public int SortOrder { get; } = -1;

		/// <inheritdoc />
		public ToolbarItemType ItemType { get; } = ToolbarItemType.Module;
	}
}