using Client.Desktop.Wpf.ViewModels.Dialogs;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Dialogs
{
    public partial class TextDialogView : ReactiveUserControl<TextDialogViewModel>
    {
        public bool? Answer { get; set; } = null;

        public TextDialogView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Title, v => v.TitleText.Text);
                this.OneWayBind(ViewModel, vm => vm.Message, v => v.MessageText.Text);
                this.BindCommand(ViewModel, vm => vm.CloseCommand, v => v.OkButton);
            });
        }

        private void OkButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Answer = true;
        }

        private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Answer = false;
        }
    }
}
