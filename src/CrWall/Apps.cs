using System;
using System.Xaml;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens.MainMenu;
using Action = System.Action;

namespace CrWall
{
    public class CertificateManagerApp : AppMainMenuBase
    {
        public CertificateManagerApp(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
        }

        public override ImageSource IconSource
            => new BitmapImage(new Uri("pack://application:,,,/UnionGas;component/Resources/dd.png"));

        public override string AppTitle => "Create Certificate";
        public override Action ClickAction => async () => await ScreenManager.ChangeScreen<CertificateCreatorViewModel>();
    }
}