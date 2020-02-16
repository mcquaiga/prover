using System;
using MvvmDialogs;
using MvvmDialogs.DialogFactories;

namespace Client.Wpf.Views.Dialogs
{
    public class DialogFactory : IDialogFactory
    {
        private readonly IServiceProvider _services;

        public DialogFactory(IServiceProvider services)
        {
            _services = services;
        }

        public IWindow Create(Type dialogType)
        {
            return (IWindow) _services.GetService(dialogType);
        }
    }
}