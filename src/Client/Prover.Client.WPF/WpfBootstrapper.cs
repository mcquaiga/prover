using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Newtonsoft.Json;
using Prover.Client.Framework;
using Prover.Client.Framework.Configuration;
using Prover.Client.Framework.Infrastructure;
using Prover.Client.Framework.Screens.Shell;
using Prover.Client.WPF.Screens.Shell;
using Prover.Shared.Data;
using Prover.Shared.Infrastructure;
using Prover.Storage.EF;
using Prover.Storage.EF.Repositories;
using Prover.Storage.EF.Storage;
using ReactiveUI.Autofac;
using IScreen = ReactiveUI.IScreen;

namespace Prover.Client.WPF
{
    public class WpfBootstrapper : ClientBootstrapperBase
    {
        protected override void RegisterDependencies(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<ShellViewModel>().As<IScreenManager>().InstancePerLifetimeScope();
            builder.RegisterType<WindowManager>().As<IWindowManager>().SingleInstance();
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            builder.RegisterForReactiveUI(Assembly.GetAssembly(this.GetType()));
        }

        protected override void DisplayStartupView()
        {
            DisplayRootViewFor<ShellViewModel>();
            Task.Run(() => ContainerManager.Resolve<IScreenManager>().GoHome());
        }
    }
}