using System.Collections.Generic;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;
using Prover.Application.Interactions;
using Prover.Application.ViewModels;

namespace Client.Desktop.Wpf.ViewModels
{
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