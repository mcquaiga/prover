using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autofac;
using Prover.Core.Exports;
using Prover.Core.Modules.Clients.VerificationTestActions;
using Prover.Core.Storage;
using Prover.Core.VerificationTests.TestActions;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens.MainMenu;
using Prover.GUI.Modules.ClientManager.Screens;
using Prover.GUI.Modules.ClientManager.Screens.CsvExporter;

namespace Prover.GUI.Modules.ClientManager
{
    public class ClientManagerModule : Module, IHaveMainMenuItem
    {
        protected ScreenManager ScreenManager { get; private set; }

        public ImageSource MenuIconSource
            => new BitmapImage(new Uri("pack://application:,,,/Prover;component/Resources/group.png"));

        public string MenuTitle => "Clients";

        public Action OpenAction => async () => await ScreenManager.ChangeScreen<ClientManagerViewModel>();
        public int Order => 2;

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
                {
                    ScreenManager = c.Resolve<ScreenManager>();
                    return this;
                })
                .As<IHaveMainMenuItem>()
                .InstancePerLifetimeScope();

            builder.Register(c => new ClientStore(c.Resolve<ProverContext>()))
                .As<IClientStore>()
                .InstancePerDependency();            
            builder.RegisterType<ItemVerificationManager>().As<IPreTestValidation>();
            builder.RegisterType<ClientPostTestResetManager>().As<IPostTestAction>();
            builder.RegisterType<ExportToCsvManager>().As<IExportCertificate>();
            builder.RegisterType<ExportToCsvViewModel>();
        }
    }
}