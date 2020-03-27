using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ReactiveUI;

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
                this.OneWayBind(ViewModel, vm => vm.DisplayName, v => v.Username).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.LogIn, v => v.LoginButton.Command).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LogOut, v => v.LogoutButton.Command).DisposeWith(d);

                ViewModel.LogIn.IsExecuting
                    .Select(x => x ? Visibility.Visible : Visibility.Collapsed)
                    .BindTo(this, view => view.ExecutingContent.Visibility);

                ViewModel.LogIn
                    .Select(x => x ? Visibility.Visible : Visibility.Collapsed)
                    .BindTo(this, view => view.LoggedInContent.Visibility);

                ViewModel.LogOut
                    .Select(x => !x ? Visibility.Visible : Visibility.Collapsed)
                    .BindTo(this, view => view.LoggedOutContent.Visibility);
            });
        }
    }
}
