using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Client.Wpf.Controls
{
    public class PercentageControlViewModel
    {
        public PercentageControlViewModel()
        {
            Passed = true;
            DisplayValue = 100.01m;

            if (Passed)
            {
                IconSource =
                    new BitmapImage(new Uri("pack://application:,,,/Prover.GUI;component/Resources/success.png"));
                IconBackground = Brushes.ForestGreen;
            }
            else
            {
                IconSource =
                    new BitmapImage(new Uri("pack://application:,,,/Prover.GUI;component/Resources/error.png"));
                IconBackground = Brushes.IndianRed;
            }
        }

        public bool Passed { get; set; }
        public decimal DisplayValue { get; set; }
        public BitmapImage IconSource { get; set; }
        public Brush IconBackground { get; set; }
    }
}