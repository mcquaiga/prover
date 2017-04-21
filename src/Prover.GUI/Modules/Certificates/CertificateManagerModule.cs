using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autofac;
using Prover.Core.Storage;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens.MainMenu;
using Prover.GUI.Modules.Certificates.Screens;

namespace Prover.GUI.Modules.Certificates
{
    public class CertificateManagerModule : Module, IHaveMainMenuItem
    {
        protected ScreenManager ScreenManager;

        public ImageSource MenuIconSource
            => new BitmapImage(new Uri("pack://application:,,,/Prover.GUI;component/Resources/certificate.png"));

        public string MenuTitle => "Create Certificates";

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
                {
                    ScreenManager = c.Resolve<ScreenManager>();
                    return this;
                })
                .As<IHaveMainMenuItem>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CertificateStore>().As<ICertificateStore>().InstancePerLifetimeScope();
        }

        public Action OpenAction => () => ScreenManager.ChangeScreen<CertificateCreatorViewModel>();
    }
}