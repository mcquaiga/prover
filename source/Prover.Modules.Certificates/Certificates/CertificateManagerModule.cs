using System;
using Autofac;
using MaterialDesignThemes.Wpf;
using Prover.Core.Exports;
using Prover.GUI.Screens.MainMenu;
using Prover.GUI.Screens.Modules.Certificates.Screens;

namespace Prover.GUI.Screens.Modules.Certificates
{
    public class CertificateManagerModule : Module, IHaveMainMenuItem
    {
        protected ScreenManager ScreenManager;

        public PackIconKind MenuIconKind => PackIconKind.ClipboardText;

        public string MenuTitle => "Certificates";

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
                {
                    ScreenManager = c.Resolve<ScreenManager>();
                    return this;
                })
                .As<IHaveMainMenuItem>()
                .SingleInstance();


            builder.RegisterType<ExportToCsvManager>()
                .As<IExportCertificate>();
        }

        public Action OpenAction => () => ScreenManager.ChangeScreen<CertificateCreatorViewModel>();
        public int Order => 4;
    }
}