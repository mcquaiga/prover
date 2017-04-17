using System.Windows.Controls;
using ReactiveUI;

namespace Prover.GUI.Common.Screens.MainMenu
{
    /// <summary>
    ///     Interaction logic for MainMenuView.xaml
    /// </summary>
    public partial class MainMenuView : UserControl, IViewFor<MainMenuViewModel>
    {
        public MainMenuView()
        {
            InitializeComponent();
        }

        public MainMenuViewModel ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MainMenuViewModel) value; }
        }
    }
}