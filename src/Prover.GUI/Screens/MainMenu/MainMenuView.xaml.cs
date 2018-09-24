using System.Windows.Controls;
using ReactiveUI;

namespace Prover.GUI.Screens.MainMenu
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

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MainMenuViewModel) value;
        }

        public MainMenuViewModel ViewModel { get; set; }
    }
}