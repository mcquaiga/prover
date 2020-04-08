using System.Reactive;
using Prover.Application.Interfaces;
using ReactiveUI;

namespace Prover.Application.ViewModels
{
    public abstract class RoutableViewModelBase : ViewModelBase, IRoutableViewModel
    {
        public IScreenManager ScreenManager { get; protected set; }

        protected RoutableViewModelBase(IScreenManager screenManager, string urlPathSegment = null)
        {
            ScreenManager = screenManager;
            HostScreen = screenManager;
            UrlPathSegment = urlPathSegment;

            CloseView = ReactiveCommand.CreateFromTask(ScreenManager.GoBack);
        }
        public ReactiveCommand<Unit, Unit> CloseView { get; protected set; }
        public string UrlPathSegment { get; protected set; }
        public IScreen HostScreen { get; protected set; }
    }
}