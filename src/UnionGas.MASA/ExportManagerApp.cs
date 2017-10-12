using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Common.Screens.MainMenu;
using UnionGas.MASA.Screens.Exporter;
using Action = System.Action;

namespace UnionGas.MASA
{
    public class ExportManagerApp : ViewModelBase, IAppMainMenu
    {
        public ExportManagerApp(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
        }
        
        public PackIconKind IconKind => PackIconKind.CloudUpload;
        public string AppTitle => "Export to MASA";
        public Action ClickAction => async () => await ScreenManager.ChangeScreen<ExportTestsViewModel>();
    }
}