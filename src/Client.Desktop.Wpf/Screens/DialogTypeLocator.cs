using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using MvvmDialogs.DialogTypeLocators;
using ReactiveUI;

namespace Client.Desktop.Wpf.Screens
{
    public class DialogTypeLocator : IDialogTypeLocator
    {
        private readonly IServiceProvider _services;
        private readonly IViewLocator _locator;

        public DialogTypeLocator(IServiceProvider services, IViewLocator locator = null)
        {
            _services = services;
            _locator = locator;
        }

        public Type Locate(INotifyPropertyChanged viewModel)
        {
            return LocateByName(viewModel);
            var viewLocator = _locator ?? ReactiveUI.ViewLocator.Current;
            var view = viewLocator.ResolveView(viewModel, "") ?? viewLocator.ResolveView(viewModel, null);

            return view.GetType();

            var assembly = Assembly.GetCallingAssembly();
            var modelType = viewModel.GetType();
            
            foreach (var ti in assembly.DefinedTypes
                .Where(ti => ti.ImplementedInterfaces.Contains(typeof(IViewFor)) && !ti.IsAbstract))
            {
                // grab the first _implemented_ interface that also implements IViewFor, this should be the expected IViewFor<>
                var ivf = ti.ImplementedInterfaces.FirstOrDefault(t =>
                    t.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IViewFor)));

                if (ivf != null && ivf.GenericTypeArguments.Contains(modelType))
                {
                    return _services.GetService(ivf).GetType();
                }
            }

            return null;
        }

        private Type LocateByName(INotifyPropertyChanged viewModel)
        {
            Type viewModelType = viewModel.GetType();
            string viewModelTypeName = viewModelType.FullName;

            // Get dialog type name by removing the 'VM' suffix
            string dialogTypeName = viewModelTypeName.Substring(0, viewModelTypeName.Length - "ViewModel".Length);
            dialogTypeName = dialogTypeName.Replace("ViewModels", "Views");
        
            return viewModelType.Assembly.GetType(dialogTypeName);
        }
    }
}