using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens.MainMenu;
using Prover.Modules.Exporter.Screens.Exporter;
using Action = System.Action;

namespace Prover.Modules.Exporter
{
    public class ExportManagerApp : AppMainMenuBase
    {
        public ExportManagerApp(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
        }

        public override ImageSource IconSource
            => new BitmapImage(new Uri("pack://application:,,,/Prover.Modules.Exporter;component/Resources/cloud-upload.png"));

        public override string AppTitle => "Export Test Runs";
        public override Action ClickAction => async () => await ScreenManager.ChangeScreen<ExportTestsViewModel>();
    }
}