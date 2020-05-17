using System.Windows.Input;
using Prover.Application.Interfaces;

namespace Prover.UI.Desktop.Controls
{
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
}