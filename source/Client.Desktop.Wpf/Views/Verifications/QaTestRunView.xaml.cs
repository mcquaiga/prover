using System.Reactive.Disposables;
using System.Windows;
using Client.Desktop.Wpf.Extensions;
using Prover.Application.VerificationManagers;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications
{
    /// <summary>
    ///     Interaction logic for VerificationTest.xaml
    /// </summary>
    public partial class QaTestRunView : IActivatableView, IViewFor<TestManagerBase>
    {
        public QaTestRunView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                ViewModel = (TestManagerBase)DataContext;
                //this.OneWayBind(DataContext, vm => vm.TestManager, v => v.EditTestToolBarControl.Visibility,
                //        value => value != null ? Visibility.Visible : Visibility.Collapsed)
                //    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.PrintTestReport, v => v.PrintButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.SubmitTest, v => v.SubmitTestButton).DisposeWith(d);


                //this.CleanUpDefaults().DisposeWith(d);
            });
        }

        //public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        //    "ViewModel", typeof(TestManagerBase), typeof(QaTestRunView), new PropertyMetadata(default(TestManagerBase)));

        //public TestManagerBase ViewModel
        //{
        //    get { return (TestManagerBase)GetValue(ViewModelProperty); }
        //    set { SetValue(ViewModelProperty, value); }
        //}

        //object IViewFor.ViewModel
        //{
        //    get => ViewModel;
        //    set => ViewModel = (TestManagerBase) value;
        //}

        //public TestManagerBase ViewModel { get; set; }
        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestManagerBase) value;
        }

        public TestManagerBase ViewModel { get; set; }
    }
}