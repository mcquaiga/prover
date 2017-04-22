using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autofac;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens.MainMenu;
using Prover.GUI.Modules.QAProver.Screens;
using Action = System.Action;

namespace Prover.GUI.Modules.QAProver
{
    public class QaTestRunModule : Module, IHaveMainMenuItem
    {
        public ImageSource MenuIconSource
            => new BitmapImage(new Uri("pack://application:,,,/Prover.GUI;component/Resources/clipboard-check.png"));

        public string MenuTitle => "New QA Test Run";

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => {ScreenManager = c.Resolve<ScreenManager>(); return this; }).As<IHaveMainMenuItem>();
        }

        public ScreenManager ScreenManager { get; set; }

        public Action OpenAction => () => ScreenManager?.ChangeScreen<TestRunViewModel>();
        public int Order => 1;
    }
}