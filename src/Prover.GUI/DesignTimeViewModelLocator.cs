using Caliburn.Micro;
using System;
using System.Linq;
using System.Windows.Data;
using System.Globalization;
using Caliburn.Micro.ReactiveUI;

namespace Prover.GUI
{
    public class DesignTimeViewModelLocator : IValueConverter
    {
        public static DesignTimeViewModelLocator Instance = new DesignTimeViewModelLocator();

        private static readonly SimpleContainer container;

        static DesignTimeViewModelLocator()
        {
            if (!Execute.InDesignMode) return;

            AssemblySource.Instance.Clear();
            AssemblySource.Instance.Add(typeof(DesignTimeViewModelLocator).Assembly);
            container = new SimpleContainer();
            IoC.GetInstance = container.GetInstance;
            IoC.GetAllInstances = container.GetAllInstances;
            IoC.BuildUp = container.BuildUp;

            var viewModels = typeof(DesignTimeViewModelLocator).Assembly.DefinedTypes
               .Where(t => t.IsSubclassOf(typeof(ReactiveScreen)));
            foreach (var vm in viewModels)
            {
                container.RegisterPerRequest(vm.AsType(), null, vm.AsType());
            }

            container.Singleton<IEventAggregator, EventAggregator>();
            
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Blend creates types from a runtime/dynamic assembly, so match on name/namespace
            var viewModelType = typeof(DesignTimeViewModelLocator).Assembly.DefinedTypes
                .First(t => t.Name == value.GetType().Name && value.GetType().Namespace.EndsWith(t.Namespace, StringComparison.Ordinal)).AsType();

            return container.GetInstance(viewModelType, null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}