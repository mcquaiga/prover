using System.Windows;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    /// <summary>
    /// Interaction logic for VerificationsGrid.xaml
    /// </summary>
    public partial class VerificationsGrid
    {
        public VerificationsGrid()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PrintDataTemplateProperty = DependencyProperty.Register(
            "PrintDataTemplate", typeof(DataTemplate), typeof(VerificationsGrid), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate PrintDataTemplate
        {
            get => (DataTemplate) GetValue(PrintDataTemplateProperty);
            set => SetValue(PrintDataTemplateProperty, value);
        }
    }
}
