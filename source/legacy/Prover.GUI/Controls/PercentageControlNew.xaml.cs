using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace Prover.GUI.Controls
{
    /// <summary>
    ///     Interaction logic for PercentageControl.xaml
    /// </summary>
    public partial class PercentageControlNew : UserControl
    {
        // Using a DependencyProperty as the backing store for IconSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register(nameof(IconSource), typeof(ImageSource), typeof(PercentageControlNew),
                new FrameworkPropertyMetadata(null));

        // Using a DependencyProperty as the backing store for IconBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconBackgroundProperty =
            DependencyProperty.Register(nameof(IconBackground), typeof(Brush), typeof(PercentageControlNew),
                new FrameworkPropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Passed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PassedProperty =
            DependencyProperty.Register(nameof(Passed), typeof(bool), typeof(PercentageControlNew),
                new FrameworkPropertyMetadata(PassedPropertyChanged));

        // Using a DependencyProperty as the backing store for DisplayValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayValueProperty =
            DependencyProperty.Register(nameof(DisplayValue), typeof(decimal?), typeof(PercentageControlNew),
                new PropertyMetadata(0.0m));

        public static readonly DependencyProperty IconKindProperty = DependencyProperty.Register(
            "IconKind", typeof(PackIconKind), typeof(PercentageControlNew),
            new PropertyMetadata(default(PackIconKind)));

        public PackIconKind IconKind
        {
            get => (PackIconKind) GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }

        public PercentageControlNew()
        {
            InitializeComponent();
            UpdateIcon(this);
        }

        public bool Passed
        {
            get => (bool) GetValue(PassedProperty);
            set => SetValue(PassedProperty, value);
        }

        public ImageSource IconSource
        {
            get => (ImageSource) GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        public Brush IconBackground
        {
            get => (Brush) GetValue(IconBackgroundProperty);
            set => SetValue(IconBackgroundProperty, value);
        }

        public decimal? DisplayValue
        {
            get => (decimal?) GetValue(DisplayValueProperty);
            set => SetValue(DisplayValueProperty, value);
        }

        public static void PassedPropertyChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var myObj = dependencyObject as PercentageControlNew;
            UpdateIcon(myObj);
        }

        private static void UpdateIcon(PercentageControlNew myObj)
        {
            if (myObj.Passed)
            {
                myObj.IconKind = PackIconKind.Check;
                myObj.IconBackground = (Brush) myObj.Resources["PassColour"];
            }
            else
            {
                myObj.IconKind = PackIconKind.AlertCircleOutline;
                myObj.IconBackground = (Brush) myObj.Resources["FailColour"];
            }
        }
    }
}