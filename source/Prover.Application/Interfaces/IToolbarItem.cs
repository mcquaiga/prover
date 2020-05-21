using System.Collections.Generic;
using System.Reactive;
using System.Windows.Input;
using ReactiveUI;

namespace Prover.Application.Interfaces
{
	public enum ToolbarItemType
	{
		Module,
		Action,
		MainMenu
	}

	public interface IToolbarItem
	{
		int SortOrder { get; }
		ToolbarItemType ItemType { get; }
	}


	//public interface IMainMenuItem : IToolbarItem
	//{
	//	string MenuIconKind { get; }
	//	string MenuTitle { get; }

	//	ReactiveCommand<Unit, Unit> OpenCommand { get; }
	//}

	public interface IToolbarButton : IToolbarItem
	{
		string Icon { get; }
		ICommand ToolbarAction { get; }
	}

	public interface IHaveToolbarActionItems
	{

	}

	public interface IHaveToolbarItems
	{
		IEnumerable<IToolbarButton> ToolbarActionItems { get; }
	}
}