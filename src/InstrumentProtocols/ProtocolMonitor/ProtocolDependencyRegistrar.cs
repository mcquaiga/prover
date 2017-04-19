using System.Windows.Media;
using Autofac;
using Caliburn.Micro;
using Prover.Client.Framework;
using Prover.Client.Framework.Screens.Shell;
using Prover.ProtocolMonitor.ViewModel;
using Prover.Shared.Infrastructure;
using Prover.Shared.Infrastructure.DependencyManagement;
using Action = System.Action;

namespace Prover.ProtocolMonitor
{
    public class ProtocolDependencyRegistrar : IDependencyRegistrar
    {
        public void Register<TConfig>(ContainerBuilder builder, ITypeFinder typeFinder, TConfig config) where TConfig : class
        {
            builder.Register(x => new ProtocolMonitorMenuItem(x.Resolve<IScreenManager>()))
                .As<IAppMainMenuItem>()
                .InstancePerLifetimeScope();
        }

        public int Order => 99;
    }

    public class ProtocolMonitorMenuItem : IAppMainMenuItem
    {
        private readonly IScreenManager _screenConductor;

        public ProtocolMonitorMenuItem(IScreenManager screenConductor)
        {
            _screenConductor = screenConductor;
        }

        public string AppTitle => "Protocol Monitor";
        public Action ClickAction => () => _screenConductor.ChangeScreen<SerialCommViewModel>();
        public ImageSource IconSource { get; }
    }
}