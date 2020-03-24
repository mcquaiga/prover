using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Prover.GUI.Controls
{
    /// <summary>
    ///     Interaction logic for InstrumentField.xaml
    /// </summary>
    public partial class ValueDescriptionControl : UserControl
    {
        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(ValueDescriptionControl));

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(ValueDescriptionControl));

        // Using a DependencyProperty as the backing store for Size.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelFontSizeProperty =
            DependencyProperty.Register(nameof(LabelFontSize), typeof(int), typeof(ValueDescriptionControl),
                new UIPropertyMetadata(16));

        // Using a DependencyProperty as the backing store for ValueFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueFontSizeProperty =
            DependencyProperty.Register(nameof(ValueFontSize), typeof(int), typeof(ValueDescriptionControl),
                new UIPropertyMetadata(20));

        // Using a DependencyProperty as the backing store for ControlBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlBackgroundProperty =
            DependencyProperty.Register(nameof(ControlBackground), typeof(Brush), typeof(ValueDescriptionControl),
                new UIPropertyMetadata(Brushes.White));

        public ValueDescriptionControl()
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

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly", typeof(bool), typeof(ValueDescriptionControl), new PropertyMetadata(true));

        public bool IsReadOnly
        {
            get => (bool) GetValue(IsReadOnlyProperty);
            set
            {
                SetValue(IsReadOnlyProperty, value);
                ShowEditSymbol = !IsReadOnly;
            }
        }

        public bool ShowEditSymbol
        {
            get => (bool) GetValue(ShowEditSymbolProperty);
            set => SetValue(ShowEditSymbolProperty, value);
        }

        // Using a DependencyProperty as the backing store for ShowEditSymbol.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowEditSymbolProperty =
            DependencyProperty.Register("ShowEditSymbol", typeof(bool), typeof(ValueDescriptionControl),
                new PropertyMetadata(false));
    }
}