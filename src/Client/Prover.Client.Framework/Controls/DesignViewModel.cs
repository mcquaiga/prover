using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Prover.Client.Framework.Controls
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
                    new BitmapImage(new Uri("pack://application:,,,/Prover.Client.Framework;component/Resources/success.png"));
                IconBackground = Brushes.ForestGreen;
            }
            else
            {
                IconSource = new BitmapImage(new Uri("pack://application:,,,/Prover.Client.Framework;component/Resources/error.png"));
                IconBackground = Brushes.IndianRed;
            }
        }

        public decimal DisplayValue { get; set; }
        public Brush IconBackground { get; set; }
        public BitmapImage IconSource { get; set; }

        public bool Passed { get; set; }
    }
}