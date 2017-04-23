using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autofac;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens.MainMenu;
using Prover.Modules.Exporter.Screens.Exporter;
using Action = System.Action;

namespace Prover.Modules.Exporter
{
    public class ExportManagerModule : Module, IHaveMainMenuItem
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => { ScreenManager = c.Resolve<ScreenManager>(); return this; })
               .As<IHaveMainMenuItem>()
               .InstancePerLifetimeScope();
        }

        public ScreenManager ScreenManager { get; set; }

        public ImageSource MenuIconSource => new BitmapImage(new Uri("pack://application:,,,/Prover.Modules.Exporter;component/Resources/cloud-upload.png"));

        public string MenuTitle => "Export Test Runs";

        public Action OpenAction => async () => await ScreenManager.ChangeScreen<ExportTestsViewModel>();
        public int Order => 3;
    }
}