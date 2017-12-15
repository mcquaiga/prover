using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autofac;
using Prover.GUI.Screens.MainMenu;
using Prover.GUI.Screens.Modules.QAProver.Screens;

namespace Prover.GUI.Screens.Modules.QAProver
{
    public class QaTestRunModule : Module, IHaveMainMenuItem
    {
        public ImageSource MenuIconSource
            => new BitmapImage(new Uri("pack://application:,,,/Prover;component/Resources/clipboard-check.png"));

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