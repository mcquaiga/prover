using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Application.ViewModels.Volume;
using ReactiveUI;

namespace Client.Wpf.Views.Verifications.Details.Volume
{
    /// <summary>
    /// Interaction logic for UncorrectedVolumeView.xaml
    /// </summary>
    public partial class UncorrectedVolumeView : ReactiveUserControl<UncorrectedVolumeTestViewModel>
    {
        public UncorrectedVolumeView()
        {
            InitializeComponent();
        }
    }
}
