using System.Reactive;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interfaces;
using ReactiveUI;

namespace Client.Desktop.Wpf.ViewModels
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