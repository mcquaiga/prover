using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Common.Screens.MainMenu;
using Prover.GUI.Screens.QAProver;
using Action = System.Action;

namespace Prover.GUI
{
    public class QaTestRunApp : ViewModelBase, IAppMainMenu
    {
        public QaTestRunApp(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
        }

        public PackIconKind IconKind => PackIconKind.ClipboardCheck;
        public string AppTitle => "New QA Test Run";

        public Action ClickAction
            => async () => await ScreenManager.ChangeScreen<TestRunViewModel>();
    }
}