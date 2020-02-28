using System.Diagnostics;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using Client.Desktop.Wpf.Extensions;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications
{
    /// <summary>
    /// Interaction logic for EditTestView.xaml
    /// </summary>
    public partial class TestDetailsView : ReactiveUserControl<EvcVerificationViewModel>
    {
        public TestDetailsView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
                {

                    this.OneWayBind(ViewModel, vm => vm.Tests, v => v.TestPointItems.ItemsSource).DisposeWith(d);
                    this.OneWayBind(ViewModel, vm => vm.DeviceInfo, v => v.SiteInfoContent.ViewModel).DisposeWith(d);
                    this.OneWayBind(ViewModel, vm => vm.VolumeTest, v => v.VolumeContentHost.ViewModel).DisposeWith(d);

                    this.CleanUpDefaults().DisposeWith(d);

                    Disposable.Create(() =>
                    {
                        TestPointItems.ItemsSource = null;
                        TestPointItems = null;
                    }).DisposeWith(d);
                });

            if (CorrectionTestsItemTemplate == null)
                CorrectionTestsItemTemplate = (DataTemplate)FindResource("CorrectionsReadOnlyDataTemplate");
        }

        public static readonly DependencyProperty CorrectionTestsItemTemplateProperty =
            DependencyProperty.Register(nameof(CorrectionTestsItemTemplate), typeof(DataTemplate), typeof(ItemsControl));

        public DataTemplate  CorrectionTestsItemTemplate
        {
            get => (DataTemplate)GetValue(CorrectionTestsItemTemplateProperty);
            set => SetValue(CorrectionTestsItemTemplateProperty, value);
        }

        public static readonly DependencyProperty VolumeTestContentTemplateProperty =
            DependencyProperty.Register(nameof(VolumeTestContentTemplate), typeof(DataTemplate), typeof(ContentControl));

        public DataTemplate  VolumeTestContentTemplate
        {
            get => (DataTemplate)GetValue(VolumeTestContentTemplateProperty);
            set => SetValue(VolumeTestContentTemplateProperty, value);
        }
    }

  
}
