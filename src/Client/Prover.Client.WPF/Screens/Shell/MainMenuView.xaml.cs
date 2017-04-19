using System.Windows.Controls;
using Prover.Client.Framework.Screens.Shell;
using ReactiveUI;

namespace Prover.Client.WPF.Screens.Shell
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