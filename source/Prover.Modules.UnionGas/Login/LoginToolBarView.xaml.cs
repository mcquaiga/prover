using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

namespace Prover.Modules.UnionGas.Login
{
    /// <summary>
    /// Interaction logic for LoginToolBarView.xaml
    /// </summary>
    public partial class LoginToolBarView
    {
        public LoginToolBarView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.DisplayName, v => v.SignOnMessageTextBlock.Text,
                value => $"Welcome, {value}").DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.LogIn, v => v.LoginButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LogOut, v => v.LogoutButton).DisposeWith(d);

                ViewModel.LogIn.IsExecuting
                    .Merge(ViewModel.LoginService.LoggedIn.Select(x => false))
                    .Select(VisibleIfTrue)
                    .BindTo(this, view => view.ExecutingContent.Visibility).DisposeWith(d);

                ViewModel.LogIn.ThrownExceptions.Subscribe(_ => ResetState());

                //ViewModel.LogIn.IsExecuting
                //         .Where(executing => executing == true)
                //         //.Select(x => )
                //         .Timeout()
                ViewModel.LoginService.LoggedIn
                         .Where(isLoggedOn => isLoggedOn)
                         .Do(_ => SignedOnPopupBox.IsPopupOpen = true)
                         .Delay(TimeSpan.FromSeconds(3))
                         .ObserveOn(RxApp.MainThreadScheduler)
                         .Subscribe(_ => SignedOnPopupBox.IsPopupOpen = false);

                ViewModel.LoginService.LoggedIn
                         .Select(VisibleIfTrue)
                         .BindTo(this, view => view.LoggedInContent.Visibility).DisposeWith(d);

                ViewModel.LoginService.LoggedIn
                    .Merge(ViewModel.LogIn.IsExecuting.Where(x => x))
                    .Select(VisibleIfFalse)
                    .BindTo(this, view => view.NotLoggedInContent.Visibility).DisposeWith(d);
            });

            ResetState();
        }

        private void ResetState()
        {
            NotLoggedInContent.Visibility = Visibility.Visible;
            LoggedInContent.Visibility = Visibility.Collapsed;
            ExecutingContent.Visibility = Visibility.Collapsed;
        }

        private Visibility VisibleIfTrue(bool value)
        {
            return value ? Visibility.Visible : Visibility.Collapsed;
        }

        private Visibility VisibleIfFalse(bool value)
        {
            return !value ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CollapseAllContentStates()
        {
            NotLoggedInContent.Visibility = Visibility.Collapsed;
            LoggedInContent.Visibility = Visibility.Collapsed;
            ExecutingContent.Visibility = Visibility.Collapsed;
        }
    }

    //public class BoolBindingTypeConverter : IBindingTypeConverter
    //{
    //    private readonly Visibility _trueValue;
    //    private readonly Visibility _falseValue;

    //    public BoolBindingTypeConverter(Visibility trueValue = Visibility.Visible,
    //        Visibility falseValue = Visibility.Collapsed)
    //    {
    //        _trueValue = trueValue;
    //        _falseValue = falseValue;
    //    }

    //    public int GetAffinityForObjects(Type fromType, Type toType) => 2;

    //    public bool TryConvert(object @from, Type toType, object conversionHint, out object result)
    //    {
    //        if (value == null)
    //            return FalseValue;

    //        if (!(value is bool))
    //            return TrueValue;

    //        return (bool)value ? TrueValue : FalseValue;
    //    }
    //}
}
