using System.Windows.Media;
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

        public override ImageSource IconSource => null;
        public override string AppTitle => "New QA Test Run";

        public override Action ClickAction
            => async () => await ScreenManager.ChangeScreen<NewQaTestRunViewModel>();
    }


    

}