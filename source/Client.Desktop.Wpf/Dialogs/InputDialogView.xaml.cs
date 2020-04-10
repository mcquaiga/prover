using System.Reactive.Disposables;
using ReactiveUI;

namespace Client.Desktop.Wpf.Dialogs
{
    /// <summary>
    /// Interaction logic for InputDialogView.xaml
    /// </summary>
    public partial class InputDialogView
    {
        public InputDialogView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.CloseCommand, v => v.OkButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.CancelCommand, v => v.CancelButton).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.InputValue, v => v.InputTextBox.Text).DisposeWith(d);

                TitleText.Text = ViewModel.Title;
                MessageText.Text = ViewModel.Message;
            });
        }

    }
}
