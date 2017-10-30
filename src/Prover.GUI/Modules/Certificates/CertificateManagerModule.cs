﻿using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autofac;
using Prover.Core.Exports;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Core.Services;
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
            => new BitmapImage(new Uri("pack://application:,,,/Prover;component/Resources/certificate.png"));

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