using System.Windows;
using System.Windows.Controls;

namespace Prover.GUI.Modules.Certificates.Common
{
    /// <summary>
    /// Interaction logic for ValueLabelControl.xaml
    /// </summary>
    public partial class ValueLabelControl : UserControl
    {
        public ValueLabelControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PropertyTypeProperty = DependencyProperty.Register(
            "PropertyType", typeof(string), typeof(ValueLabelControl), new PropertyMetadata(default(string)));

        public string PropertyType
        {
            get => (string) GetValue(PropertyTypeProperty);
            set => SetValue(PropertyTypeProperty, value);
        }
    }
}