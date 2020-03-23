using Prover.Application.Interfaces;
using ReactiveUI;

namespace Prover.Application.ViewModels
{
    public abstract class RoutableViewModelBase : ViewModelBase, IRoutableViewModel
    {
        public IScreenManager ScreenManager { get; }

        protected RoutableViewModelBase(IScreenManager screenManager)
        {
            ScreenManager = screenManager;

        }

        public abstract string UrlPathSegment { get; } 
        public abstract IScreen HostScreen { get; }
    }
}