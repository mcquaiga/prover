using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autofac;
using Prover.Core.Models.Clients;
using Prover.Core.Modules.Clients.VerificationTestActions;
using Prover.Core.Storage;
using Prover.Core.VerificationTests.TestActions;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens.MainMenu;
using Prover.GUI.Modules.Clients.Screens.Clients;

namespace Prover.GUI.Modules.Clients
{
    public class ClientManagerModule : Module, IHaveMainMenuItem
    {
        readonly IScreenManager _screenManager;

        public ClientManagerModule() { }

        public ClientManagerModule(IScreenManager screenManager)
        {
            _screenManager = screenManager;
        }

        public ImageSource MenuIconSource
            => new BitmapImage(new Uri("pack://application:,,,/Prover.GUI;component/Resources/group.png"));

        public string MenuTitle => "Manage Clients";

        public Action MenuStartAction => () => _screenManager.ChangeScreen<ClientManagerViewModel>();

       

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new ClientManagerModule(c.Resolve<ScreenManager>()))
                .As<IHaveMainMenuItem>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ClientStore>().As<IProverStore<Client>>();
            builder.RegisterType<ItemVerificationManager>().As<IPreTestValidation>();
            builder.RegisterType<ClientPostTestResetManager>().As<IPostTestAction>();
        }
    }
}