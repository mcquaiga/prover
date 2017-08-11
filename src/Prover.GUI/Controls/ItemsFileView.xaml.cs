using System.Windows;
using System.Windows.Controls;
using ReactiveUI;

namespace Prover.GUI.Controls
{
    /// <summary>
    /// Interaction logic for ItemsFileView.xaml
    /// </summary>
    public partial class ItemsFileView : UserControl, IViewFor<ItemsFileViewModel>
    {
        public ItemsFileView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register(
            "HeaderText", typeof(string), typeof(ItemsFileView), new PropertyMetadata(default(string)));

        public string HeaderText
        {
            get { return (string) GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public static readonly DependencyProperty DescriptionTextProperty = DependencyProperty.Register(
            "DescriptionText", typeof(string), typeof(ItemsFileView), new PropertyMetadata(default(string)));

        public string DescriptionText
        {
            get { return (string) GetValue(DescriptionTextProperty); }
            set { SetValue(DescriptionTextProperty, value); }
        }


        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ItemsFileViewModel) value; }
        }

        public ItemsFileViewModel ViewModel { get; set; }
    }
}
