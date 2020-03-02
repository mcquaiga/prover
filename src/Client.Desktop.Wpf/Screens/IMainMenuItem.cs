using Client.Desktop.Wpf.ViewModels;
using MaterialDesignThemes.Wpf;
using ReactiveUI;

namespace Client.Desktop.Wpf.Screens
{
    public interface IMainMenuItem
    {
        PackIconKind MenuIconKind { get; }
        string MenuTitle { get; }

        ReactiveCommand<IScreenManager, IRoutableViewModel> OpenCommand { get; }

        int? Order { get; }
    }
}