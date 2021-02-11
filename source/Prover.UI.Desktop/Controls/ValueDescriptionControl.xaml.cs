using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Prover.UI.Desktop.Controls {
	/// <summary>
	///     Interaction logic for InstrumentField.xaml
	/// </summary>
	public partial class ValueDescriptionControl : UserControl {
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

		public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
			"IsReadOnly", typeof(bool), typeof(ValueDescriptionControl), new PropertyMetadata(true));

		// Using a DependencyProperty as the backing store for ShowEditSymbol.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ShowEditSymbolProperty =
			DependencyProperty.Register("ShowEditSymbol", typeof(bool), typeof(ValueDescriptionControl),
				new PropertyMetadata(false));

		public ValueDescriptionControl() {
			InitializeComponent();

			//EventManager.RegisterClassHandler(typeof(TextBox),
			//    GotFocusEvent,
			//    new RoutedEventHandler(TextBox_GotFocus));

			//EventManager.RegisterClassHandler(typeof(TextBox), PreviewMouseLeftButtonDownEvent,
			//    new MouseButtonEventHandler(SelectivelyIgnoreMouseButton));
			//EventManager.RegisterClassHandler(typeof(TextBox), GotKeyboardFocusEvent,
			//    new RoutedEventHandler(SelectAllText));
			//EventManager.RegisterClassHandler(typeof(TextBox), MouseDoubleClickEvent,
			//    new RoutedEventHandler(SelectAllText));
		}

		public string Label
		{
			get => (string)GetValue(LabelProperty);
			set => DispatcherSetValue(LabelProperty, value);
		}

		public string Value
		{
			get => (string)GetValue(ValueProperty);
			set => DispatcherSetValue(ValueProperty, value);
		}

		public int LabelFontSize
		{
			get => (int)GetValue(LabelFontSizeProperty);
			set => DispatcherSetValue(LabelFontSizeProperty, value);
		}

		public int ValueFontSize
		{
			get => (int)GetValue(ValueFontSizeProperty);
			set => DispatcherSetValue(ValueFontSizeProperty, value);
		}

		public Brush ControlBackground
		{
			get => (Brush)GetValue(ControlBackgroundProperty);
			set => DispatcherSetValue(ControlBackgroundProperty, value);
		}

		public bool IsReadOnly
		{
			get => (bool)GetValue(IsReadOnlyProperty);
			set {
				DispatcherSetValue(IsReadOnlyProperty, value);
				ShowEditSymbol = !IsReadOnly;
			}
		}

		public bool ShowEditSymbol
		{
			get => (bool)GetValue(ShowEditSymbolProperty);
			set => DispatcherSetValue(ShowEditSymbolProperty, value);
		}

		private static void SelectAllText(object sender, RoutedEventArgs e) {
			var textBox = e.OriginalSource as TextBox;
			if (textBox != null)
				textBox.SelectAll();
		}

		private static void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e) {
			// Find the TextBox
			DependencyObject parent = e.OriginalSource as UIElement;
			while (parent != null && !(parent is TextBox))
				parent = VisualTreeHelper.GetParent(parent);

			if (parent != null) {
				var textBox = (TextBox)parent;
				if (!textBox.IsKeyboardFocusWithin) {
					// If the text box is not yet focused, give it the focus and
					// stop further processing of this click event.
					textBox.Focus();
					e.Handled = true;
				}
			}
		}

		private void DispatcherSetValue(DependencyProperty prop, object value) {
			SetValue(prop, value);
		}

		//private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
		//{
		//    (sender as TextBox)?.SelectAll();
		//}
	}
}