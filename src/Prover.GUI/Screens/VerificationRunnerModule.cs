using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autofac;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens.MainMenu;
using Prover.GUI.Screens.QAProver;
using Action = System.Action;

namespace Prover.GUI.Screens
{
    public class QaTestRunModule : Module, IHaveMainMenuItem
    {
        public ImageSource MenuIconSource
            => new BitmapImage(new Uri("pack://application:,,,/Prover.GUI;component/Resources/clipboard-check.png"));

        public string MenuTitle => "New QA Test Run";

        public Action MenuStartAction { get; private set; }

        protected override void Load(ContainerBuilder builder)
        {
            MenuStartAction = () => builder.Register(c => c.Resolve<IScreenManager>().ChangeScreen<TestRunViewModel>());
            builder.RegisterInstance(this).As<IHaveMainMenuItem>();
        }
    }
}