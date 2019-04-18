using System;
using Autofac;
using MaterialDesignThemes.Wpf;
using Prover.Core.Exports;
using Prover.Core.Modules.Clients.VerificationTestActions;
using Prover.Core.VerificationTests.PreTestActions;
using Prover.Core.VerificationTests.TestActions;
using Prover.GUI.Screens.MainMenu;
using Prover.GUI.Screens.Modules.ClientManager.Screens;
using Prover.GUI.Screens.Modules.ClientManager.Screens.CsvExporter;

namespace Prover.GUI.Screens.Modules.ClientManager
{
    public class ClientManagerModule : Module, IHaveMainMenuItem
    {
        protected ScreenManager ScreenManager { get; private set; }

        public PackIconKind MenuIconKind => PackIconKind.AccountMultiple;

        public string MenuTitle => "Clients";
        public Action OpenAction => () => ScreenManager.ChangeScreen<ClientManagerViewModel>();
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

            builder.RegisterType<DateTimeValidator>().As<IPreTestValidation>();
            builder.RegisterType<ItemVerificationManager>().As<IPreTestValidation>();
            builder.RegisterType<ClientPostTestResetManager>().As<IPostTestAction>();
            builder.RegisterType<ExportToCsvManager>().As<IExportCertificate>();
            builder.RegisterType<ExportToCsvViewModel>();
        }
    }
}