using System.Linq;
using System.Reactive.Disposables;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Corrections;
using ReactiveUI;
using Client.Desktop.Wpf.Extensions;

namespace Client.Desktop.Wpf.Views.Verifications.Details
{
    /// <summary>
    /// Interaction logic for CorrectionTestPointView.xaml
    /// </summary>
    public partial class CorrectionTestPointView : ReactiveUserControl<VerificationTestPointViewModel>
    {
        public CorrectionTestPointView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
                {
                    this.OneWayBind(ViewModel, vm => vm.TestNumber, v => v.TestLevelBlock.Text, value => $"Level {value+1}").DisposeWith(d);

                    this.OneWayBind(ViewModel, vm => vm.TestsCollection, v => v.TestItems.ItemsSource, 
                        tests => tests.Where(t =>
                        {
                            var isOf = t.IsTypeOrInheritsOf(typeof(CorrectionTestViewModel<>));
                            var baseType = t.GetType().BaseType;

                            if (baseType != null && (baseType.IsGenericType || baseType.IsGenericTypeDefinition))
                            {
                                var success = baseType.GetGenericTypeDefinition() == typeof(CorrectionTestViewModel<>);
                                return success;
                            }
                            return false;
                        })).DisposeWith(d);

                    this.CleanUpDefaults().DisposeWith(d);

                    Disposable.Create(() =>
                    {
                        TestItems.ItemsSource = null;
                        TestItems = null;
                    }).DisposeWith(d);
                });

            
        }
    }
}
