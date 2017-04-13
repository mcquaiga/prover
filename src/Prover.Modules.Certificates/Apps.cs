using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Prover.Modules.CertificatesUi.Screens.Certificates;
using Action = System.Action;

namespace Prover.Modules.CertificatesUi
{  
    public class CertificatesApp : AppMainMenuBase
    {
        public CertificatesApp(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
        }

        public override ImageSource IconSource
            => new BitmapImage(new Uri("pack://application:,,,/Prover.Modules.Certificates;component/Resources/group.png"));

        public override string AppTitle => "Create Certificates";
        public override Action ClickAction => async () => await ScreenManager.ChangeScreen<CertificateViewModel>();
    }
}