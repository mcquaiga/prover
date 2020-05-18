using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Prover.Application.Interfaces;
using Prover.UI.Desktop.ViewModels.Verifications;
using ReactiveUI;

namespace Prover.UI.Desktop.ViewModels
{
	//public class VerificationsMainMenu : IToolbarButton
	//{
	//	public VerificationsMainMenu(IScreenManager screenManager)
	//	{
	//		OpenCommand = ReactiveCommand.CreateFromObservable(() =>
	//		{
	//			screenManager.DialogManager.Show<NewTestRunViewModel>();
	//			return Observable.Return(Unit.Default);
	//		});
	//	}

	//	public string MenuIconKind { get; } = PackIconKind.ClipboardCheck.ToString();
	//	public string MenuTitle { get; } = "New QA Test Run";
	//	public int? Order { get; } = 1;

	//	public ReactiveCommand<Unit, Unit> OpenCommand { get; }

	//	/// <inheritdoc />
	//	public int SortOrder { get; } = 1;

	//	/// <inheritdoc />
	//	public ToolbarItemType ItemType { get; } = ToolbarItemType.MainMenu;

	//	/// <inheritdoc />
	//	public string Icon { get; } = PackIconKind.ClipboardCheck.ToString();

	//	/// <inheritdoc />
	//	public ICommand ToolbarAction => OpenCommand;
	//}
}

