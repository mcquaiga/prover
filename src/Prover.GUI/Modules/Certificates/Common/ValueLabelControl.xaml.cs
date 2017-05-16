using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            get { return (string) GetValue(PropertyTypeProperty); }
            set { SetValue(PropertyTypeProperty, value); }
        }
    }
}
