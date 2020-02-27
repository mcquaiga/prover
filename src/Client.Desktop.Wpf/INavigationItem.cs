using System.Reactive;
using MaterialDesignThemes.Wpf;
using ReactiveUI;

namespace Client.Desktop.Wpf
{
    public interface INavigationItem
    {
        ReactiveCommand<Unit, Unit> NavigationCommand { get; }
        PackIconKind IconKind { get; }
        bool IsHome { get; }
    }
}
