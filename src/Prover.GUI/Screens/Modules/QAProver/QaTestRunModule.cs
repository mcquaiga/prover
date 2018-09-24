using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autofac;
using MaterialDesignThemes.Wpf;
using Prover.GUI.Screens.MainMenu;
using Prover.GUI.Screens.Modules.QAProver.Screens;

namespace Prover.GUI.Screens.Modules.QAProver
{
    public class QaTestRunModule : Module, IHaveMainMenuItem
    {
        public PackIconKind MenuIconKind => PackIconKind.ClipboardCheck;
        public string MenuTitle => "New QA Test Run";

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                ScreenManager = c.Resolve<ScreenManager>();
                return this;
            }).As<IHaveMainMenuItem>();
        }

        public ScreenManager ScreenManager { get; set; }
        public Action OpenAction => () => ScreenManager?.ChangeScreen<TestRunViewModel>();
        public int Order => 1;
    }
}