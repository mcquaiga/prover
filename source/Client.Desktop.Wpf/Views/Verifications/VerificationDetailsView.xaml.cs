using System.Linq;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using Client.Desktop.Wpf.Extensions;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications
{
    /// <summary>
    ///     Interaction logic for EditTestView.xaml
    /// </summary>
    public partial class VerificationDetailsView : ReactiveUserControl<EvcVerificationViewModel>
    {
        public static readonly DependencyProperty CorrectionTestsItemTemplateProperty =
            DependencyProperty.Register(nameof(CorrectionTestsItemTemplate), typeof(DataTemplate),
                typeof(ItemsControl));

        public static readonly DependencyProperty VolumeTestContentTemplateProperty =
            DependencyProperty.Register(nameof(VolumeTestContentTemplate), typeof(DataTemplate),
                typeof(ContentControl));

        public VerificationDetailsView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.VerificationTests, v => v.TestPointItems.ItemsSource, 
                    value => value.OfType<VerificationTestPointViewModel>()).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.DeviceInfo, v => v.SiteInfoContent.ViewModel).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.VolumeTest, v => v.VolumeContentHost.ViewModel).DisposeWith(d);

                //this.OneWayBind(ViewModel, vm => vm.VerificationTests, v => v.CustomVerificationContent.ViewModel,
                //    value => value.FirstOrDefault(x => !x.IsTypeOf(typeof(VerificationTestPointViewModel)))).DisposeWith(d);

                this.CleanUpDefaults().DisposeWith(d);
                Disposable.Create(() =>
                {
                    if (TestPointItems != null)
                        TestPointItems.ItemsSource = null;

                    TestPointItems = null;
                }).DisposeWith(d);
            });
        }

        public DataTemplate CorrectionTestsItemTemplate
        {
            get => (DataTemplate) GetValue(CorrectionTestsItemTemplateProperty);
            set => SetValue(CorrectionTestsItemTemplateProperty, value);
        }

        public DataTemplate VolumeTestContentTemplate
        {
            get => (DataTemplate) GetValue(VolumeTestContentTemplateProperty);
            set => SetValue(VolumeTestContentTemplateProperty, value);
        }
    }
}