using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autofac;
using Caliburn.Micro;
using Prover.Client.Framework;
using Prover.Client.Framework.Screens.Shell;
using Prover.Shared.Infrastructure;
using Prover.Shared.Infrastructure.DependencyManagement;
using Action = System.Action;

namespace Prover.Client.WPF.Apps.Prover
{
    public class ProverDependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 1;

        public void Register<TConfig>(ContainerBuilder builder, ITypeFinder typeFinder, TConfig config) where TConfig : class
        {
            builder.Register(x => new QaTestRunApp(x.Resolve<IScreenManager>())).As<IAppMainMenuItem>();
        }

        public class QaTestRunApp : IAppMainMenuItem
        {
            public IScreenManager ScreenManager { get; }

            public QaTestRunApp(IScreenManager screenManager)
            {
                ScreenManager = screenManager;
            }

            public string AppTitle => "New QA Test Run";

            public Action ClickAction => delegate {  } ;// () => ScreenManager.ChangeScreen<>();

            public ImageSource IconSource
                => new BitmapImage(new Uri("pack://application:,,,/Prover.Client.WPF;component/Resources/clipboard-check.png"));
        }
    }
}