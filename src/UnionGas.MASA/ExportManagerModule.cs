using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autofac;
using MaterialDesignThemes.Wpf;
using Prover.GUI.Screens;
using Prover.GUI.Screens.MainMenu;
using UnionGas.MASA.Screens.Exporter;
using Action = System.Action;

namespace UnionGas.MASA
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

        public PackIconKind MenuIconKind => PackIconKind.CloudUpload;
        public string MenuTitle => "Export Test Runs";

        public Action OpenAction => () => ScreenManager.ChangeScreen<ExportTestsViewModel>();
        public int Order => 3;
    }
}