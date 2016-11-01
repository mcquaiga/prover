using System;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens.MainMenu;
using Prover.GUI.Screens.QAProver;
using Action = System.Action;

namespace Prover.GUI
{
    public class QaTestRunApp : AppMainMenuBase
    {
        public QaTestRunApp(ScreenManager screenManager, IEventAggregator eventAggregator) : base(screenManager, eventAggregator)
        {
        }

        public override ImageSource IconSource
            => new BitmapImage(new Uri("pack://application:,,,/Prover.GUI;component/Resources/clipboard-check.png"));

        public override string AppTitle => "New QA Test Run";

        public override Action ClickAction
            => async () => await ScreenManager.ChangeScreen<NewQaTestRunViewModel>();
    }
}