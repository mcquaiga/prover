using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


namespace Prover.GUI.Controls
{
    /// <summary>
    /// Interaction logic for InstrumentField.xaml
    /// </summary>
    public partial class ValueDescriptionControl : UserControl
    {
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(ValueDescriptionControl));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(ValueDescriptionControl));

        public decimal ScaleFactor
        {
            get { return (int)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Size.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("ScaleFactor", typeof(decimal), typeof(ValueDescriptionControl), new UIPropertyMetadata(1.0m));

        public decimal ValueSize => ScaleFactor * 24;

        public decimal LabelSize => ScaleFactor * 16;

        public decimal HeightSscale => ScaleFactor * 65;

        public decimal WidthScale => ScaleFactor * 180;

        public ValueDescriptionControl()
        {
            InitializeComponent();
        }
    }
}
