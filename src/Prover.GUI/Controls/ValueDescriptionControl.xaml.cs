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
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(ValueDescriptionControl));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(ValueDescriptionControl));

        public int LabelFontSize
        {
            get { return (int)GetValue(LabelFontSizeProperty); }
            set { SetValue(LabelFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Size.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelFontSizeProperty =
            DependencyProperty.Register(nameof(LabelFontSize), typeof(int), typeof(ValueDescriptionControl), new UIPropertyMetadata(12));

        public int ValueFontSize
        {
            get { return (int)GetValue(ValueFontSizeProperty); }
            set { SetValue(ValueFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueFontSizeProperty =
            DependencyProperty.Register(nameof(ValueFontSize), typeof(int), typeof(ValueDescriptionControl), new UIPropertyMetadata(24));



        public Brush ControlBackground
        {
            get { return (Brush)GetValue(ControlBackgroundProperty); }
            set { SetValue(ControlBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ControlBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlBackgroundProperty =
            DependencyProperty.Register(nameof(ControlBackground), typeof(Brush), typeof(ValueDescriptionControl), new UIPropertyMetadata(Brushes.White));

        public ValueDescriptionControl()
        {
            InitializeComponent();
        }
    }
}
