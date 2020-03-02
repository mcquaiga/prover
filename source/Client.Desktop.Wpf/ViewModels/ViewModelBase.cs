using Prover.Application.ViewModels;
using ReactiveUI;

namespace Client.Desktop.Wpf.ViewModels
{
    public abstract class RoutableViewModelBase : ViewModelBase, IActivatableViewModel, IRoutableViewModel
    {
        public IScreenManager ScreenManager { get; }

        protected RoutableViewModelBase(IScreenManager screenManager)
        {
            ScreenManager = screenManager;

            //Activator.Activated
            //    .Delay(TimeSpan.FromMilliseconds(500))
            //    .LogDebug($"Registered VMs - {ViewModelRegisteration.ViewModels.Count}")
            //    .Subscribe();

            //Activator.Deactivated
            //    //.Delay(TimeSpan.FromMilliseconds(500))
            //    //.LogDebug($"Registered VMs - {ViewModelRegisteration.ViewModels.Count}")
            //    .Subscribe();
        }

        public abstract string UrlPathSegment { get; } 
        public abstract IScreen HostScreen { get; }
    }
}