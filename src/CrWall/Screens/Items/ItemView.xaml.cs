using System.Windows.Controls;
using ReactiveUI;

namespace CrWall.Screens.Items
{
    /// <summary>
    /// Interaction logic for ItemView.xaml
    /// </summary>
    public partial class ItemView : UserControl, IViewFor<ItemViewModel>
    {
        public ItemView()
        {
            InitializeComponent();
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ItemViewModel) value; }
        }

        public ItemViewModel ViewModel { get; set; }
    }
}
