using System.Collections.Generic;
using System.Windows.Input;

namespace Prover.Application.Interfaces
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

	public interface IHaveToolbarActionItems
	{

	}

	public interface IHaveToolbarItems
	{
		IEnumerable<IToolbarActionItem> ToolbarActionItems { get; }
	}
}