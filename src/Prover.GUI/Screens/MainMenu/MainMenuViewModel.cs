using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using Action = System.Action;

namespace Prover.GUI.Screens.MainMenu
{
    public class MainMenuViewModel : ViewModelBase, INavigationItem
    {
        public MainMenuViewModel(IEnumerable<IHaveMainMenuItem> appMainMenus, ScreenManager screenManager,
            IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
            AppMainMenus = appMainMenus.OrderBy(x => x.Order);
        }

        public IEnumerable<IHaveMainMenuItem> AppMainMenus { get; }
        public ReactiveCommand OpenModuleCommand { get; set; }

        public async Task ActionCommand(Action openAction)
        {
            await Task.Run(openAction);
        }

        public ReactiveCommand<Unit, Unit> NavigationCommand => ReactiveCommand.Create(() => ScreenManager.ChangeScreen(this));
        public PackIconKind IconKind => PackIconKind.Home;
        public bool IsHome => true;
    }
}