using System.Reactive;
using Client.Wpf.ViewModels;
using MaterialDesignThemes.Wpf;
using ReactiveUI;

namespace Client.Wpf.Screens
{
    public abstract class MainMenuItem : IMainMenuItem
    {
        protected readonly IScreenManager ScreenManager;

        protected MainMenuItem(IScreenManager screenManager, PackIconKind menuIconKind, string menuTitle, int order)
        {
            ScreenManager = screenManager;
            MenuIconKind = menuIconKind;
            MenuTitle = menuTitle;
            Order = order;
        }

        public PackIconKind MenuIconKind { get; }
        public string MenuTitle { get; }
        public abstract ReactiveCommand<Unit, IRoutableViewModel> OpenCommand { get; }
        public int Order { get; }
    }
}