using System.Reactive;
using Prover.Application.Interfaces;
using ReactiveUI;

namespace Prover.UI.Desktop.ViewModels
{
    public abstract class RoutableViewModelBase : ViewModelWpfBase, IRoutableViewModel
    {
        protected RoutableViewModelBase(IScreenManager screenManager, string urlPathSegment = null)
        {
            ScreenManager = screenManager;
            HostScreen = screenManager;
            UrlPathSegment = urlPathSegment;

            CloseView = ReactiveCommand.CreateFromTask(ScreenManager.GoBack);
        }


        public IScreenManager ScreenManager { get; protected set; }
        public ReactiveCommand<Unit, Unit> CloseView { get; protected set; }
        public string UrlPathSegment { get; protected set; }
        public IScreen HostScreen { get; protected set; }
    }
}