using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Prover.GUI.Controls
{
    /// <summary>
    ///     Interaction logic for InstrumentField.xaml
    /// </summary>
    public partial class LabelValueControl : UserControl
    {
        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(LabelValueControl));

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(LabelValueControl));

        // Using a DependencyProperty as the backing store for Size.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelFontSizeProperty =
            DependencyProperty.Register(nameof(LabelFontSize), typeof(int), typeof(LabelValueControl),
                new UIPropertyMetadata(12));

        // Using a DependencyProperty as the backing store for ValueFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueFontSizeProperty =
            DependencyProperty.Register(nameof(ValueFontSize), typeof(int), typeof(LabelValueControl),
                new UIPropertyMetadata(24));

        // Using a DependencyProperty as the backing store for ControlBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlBackgroundProperty =
            DependencyProperty.Register(nameof(ControlBackground), typeof(Brush), typeof(LabelValueControl),
                new UIPropertyMetadata(Brushes.White));

        public LabelValueControl()
        {
            InitializeComponent();
        }

        public string Label
        {
            get => (string) GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public string Value
        {
            get => (string) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public int LabelFontSize
        {
            get => (int) GetValue(LabelFontSizeProperty);
            set => SetValue(LabelFontSizeProperty, value);
        }

        public int ValueFontSize
        {
            get => (int) GetValue(ValueFontSizeProperty);
            set => SetValue(ValueFontSizeProperty, value);
        }

        public Brush ControlBackground
        {
            get => (Brush) GetValue(ControlBackgroundProperty);
            set => SetValue(ControlBackgroundProperty, value);
        }
    }
}