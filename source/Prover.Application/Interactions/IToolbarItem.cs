using System.Collections.Generic;
using System.Windows.Input;

namespace Prover.Application.Interactions
{
	public interface IToolbarItem
	{
		int SortOrder { get; }
	}


	public interface IModuleToolbarItem : IToolbarItem
	{

	}

	public interface IToolbarActionItem : IToolbarItem
	{
		string Icon { get; }
		ICommand ToolbarAction { get; }
	}

	public class ToolbarActionItem : IToolbarActionItem
	{
		public ToolbarActionItem(string icon, ICommand toolbarAction)
		{
			Icon = icon;
			ToolbarAction = toolbarAction;
		}

		/// <inheritdoc />
		public string Icon { get; set; }

		/// <inheritdoc />
		public ICommand ToolbarAction { get; set; }

		/// <inheritdoc />
		public int SortOrder { get; } = 50;
	}

	public interface IHaveToolbarActionItems
	{

	}

	public interface IHaveToolbarItems
	{
		IEnumerable<IToolbarActionItem> ToolbarActionItems { get; }
	}
}