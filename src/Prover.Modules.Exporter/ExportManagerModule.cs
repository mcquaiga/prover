using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Caliburn.Micro;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens.MainMenu;
using Prover.Modules.Exporter.Screens.Exporter;
using Action = System.Action;
using IModule = Autofac.Core.IModule;

namespace Prover.Modules.Exporter
{
    public class ExportManagerModule : Module, IHaveMainMenuItem
    {
        protected override void Load(ContainerBuilder builder)
        {
            MenuStartAction = () => builder.Register(c => c.Resolve<IScreenManager>().ChangeScreen<ExportTestsViewModel>());
            builder.RegisterInstance(this).As<IHaveMainMenuItem>();
        }

        public ImageSource MenuIconSource => new BitmapImage(new Uri("pack://application:,,,/Prover.Modules.Exporter;component/Resources/cloud-upload.png"));

        public string MenuTitle => "Export Test Runs";
        public Action MenuStartAction { get; private set; }

    }
}