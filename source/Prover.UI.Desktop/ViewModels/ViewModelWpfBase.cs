using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;
using Prover.Application.Interactions;
using Prover.Application.ViewModels;

namespace Prover.UI.Desktop.ViewModels
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

        protected void AddToolbarItem(IToolbarActionItem toolbarItem)
        {
            _toolbarActionItems.Add(toolbarItem);
        }

        protected void AddToolbarItem(IEnumerable<IToolbarActionItem> toolbarItems)
        {
            toolbarItems.ForEach(AddToolbarItem);
        }

        /// <inheritdoc />
        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                
            }

            base.Dispose(isDisposing);
        }
    }
}