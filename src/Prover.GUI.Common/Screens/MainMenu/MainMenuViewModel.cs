using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using ReactiveUI;
using Action = System.Action;

namespace Prover.GUI.Common.Screens.MainMenu
{
    public class MainMenuViewModel : ViewModelBase
    {
        private readonly ScreenManager _screenManager;

        public MainMenuViewModel(IEnumerable<IHaveMainMenuItem> appMainMenus, ScreenManager screenManager, IEventAggregator eventAggregator) 
            : base(screenManager, eventAggregator)
        {
            _screenManager = screenManager;
            AppMainMenus = appMainMenus.OrderBy(x => x.Order);
        }

        public IEnumerable<IHaveMainMenuItem> AppMainMenus { get; }

        public ReactiveCommand OpenModuleCommand { get; set; }

        public async Task ActionCommand(Action openAction)
        {
            await Task.Run(openAction);
        }
    }
}