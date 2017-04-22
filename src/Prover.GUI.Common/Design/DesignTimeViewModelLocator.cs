using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using Autofac;
using Autofac.Core;
using Prover.GUI.Common.Screens;

namespace Prover.GUI.Common.Design
{
    public class DesignTimeViewModelLocator : IValueConverter
    {
        public static DesignTimeViewModelLocator Instance = new DesignTimeViewModelLocator();

        private static readonly IContainer Container;

        static DesignTimeViewModelLocator()
        {
            if (!Execute.InDesignMode) return;

            AssemblySource.Instance.Clear();
            AssemblySource.Instance.Add(typeof(DesignTimeViewModelLocator).Assembly);
            var builder = new ContainerBuilder();

            var viewModels = typeof(DesignTimeViewModelLocator).Assembly.DefinedTypes
               .Where(t => t.IsSubclassOf(typeof(ViewModelBase)));
            foreach (var vm in viewModels)
            {
                builder.RegisterType(vm.AsType());
            }

            builder.RegisterType<EventAggregator>().As<EventAggregator>().SingleInstance();

            Container = builder.Build();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Blend creates types from a runtime/dynamic assembly, so match on name/namespace
            var viewModelType = typeof(DesignTimeViewModelLocator).Assembly.DefinedTypes
                .First(t => t.Name == value.GetType().Name && value.GetType().Namespace.EndsWith(t.Namespace)).AsType();

            return Container.Resolve(viewModelType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}