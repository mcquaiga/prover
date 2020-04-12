using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
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

    public abstract class ViewModelWpfBase : ViewModelBase
    {
        private readonly ICollection<IToolbarActionItem> _toolbarActionItems = new List<IToolbarActionItem>();

        protected ViewModelWpfBase(ILogger<ViewModelWpfBase> logger = null) : base(logger)
        {

        }
        /// <inheritdoc />
        public IEnumerable<IToolbarActionItem> ToolbarActionItems => _toolbarActionItems;
        
        protected void AddToolbarItem(ICommand command, PackIconKind iconKind)
        {
            _toolbarActionItems.Add(new ToolbarActionItem(iconKind.ToString(), command));
        }
        
    }
}