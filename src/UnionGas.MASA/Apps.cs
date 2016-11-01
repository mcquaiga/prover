using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens.MainMenu;
using UnionGas.MASA.Screens.Exporter;
using Action = System.Action;

namespace UnionGas.MASA
{
    public class ExportManagerApp : AppMainMenuBase
    {
        public ExportManagerApp(ScreenManager screenManager, IEventAggregator eventAggregator) : base(screenManager, eventAggregator)
        {
        }

        public override ImageSource IconSource
            => new BitmapImage(new Uri("pack://application:,,,/UnionGas.MASA;component/Resources/cloud-upload.png"));

        public override string AppTitle => "Export to MASA";
        public override Action ClickAction => async () => await ScreenManager.ChangeScreen<ExportTestsViewModel>();
    }
}
