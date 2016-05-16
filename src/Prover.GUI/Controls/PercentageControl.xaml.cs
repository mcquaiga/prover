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

namespace Prover.GUI.Controls
{
    /// <summary>
    /// Interaction logic for PercentageControl.xaml
    /// </summary>
    public partial class PercentageControl : UserControl
    {
        public bool Passed
        {
            get { return (bool)GetValue(PassedProperty); }
            set
            {
                SetValue(PassedProperty, value);
            }
        }

        public static void PassedPropertyChanged(DependencyObject dependencyObject,
                DependencyPropertyChangedEventArgs e)
        {
            var myObj = dependencyObject as PercentageControl;
            UpdateIcon(myObj);
        }

        private static void UpdateIcon(PercentageControl myObj)
        {
            if (myObj.Passed)
            {
                myObj.IconSource = myObj.Resources["PassIcon"] as ImageSource;
                myObj.IconBackground = (Brush)myObj.Resources["PassColour"];
            }
            else
            {
                myObj.IconSource = myObj.Resources["FailIcon"] as ImageSource;
                myObj.IconBackground = (Brush)myObj.Resources["FailColour"];
            }
        }

        public ImageSource IconSource
        {
            get { return (ImageSource)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register(nameof(IconSource), typeof(ImageSource), typeof(PercentageControl), new FrameworkPropertyMetadata(null));

        public Brush IconBackground
        {
            get { return (Brush)GetValue(IconBackgroundProperty); }
            set { SetValue(IconBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconBackgroundProperty =
            DependencyProperty.Register(nameof(IconBackground), typeof(Brush), typeof(PercentageControl), new FrameworkPropertyMetadata(null));



        // Using a DependencyProperty as the backing store for Passed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PassedProperty =
            DependencyProperty.Register(nameof(Passed), typeof(bool), typeof(PercentageControl), new FrameworkPropertyMetadata(PassedPropertyChanged));
        
      
        public decimal? DisplayValue
        {
            get { return (decimal?)GetValue(DisplayValueProperty); }
            set { SetValue(DisplayValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayValueProperty =
            DependencyProperty.Register(nameof(DisplayValue), typeof(decimal?), typeof(PercentageControl), new PropertyMetadata(0.0m));


        public PercentageControl()
        {
            InitializeComponent();
            UpdateIcon(this);
        }
    }
}
