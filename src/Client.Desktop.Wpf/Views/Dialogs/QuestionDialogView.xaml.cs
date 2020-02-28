using Client.Desktop.Wpf.Screens;
using Client.Desktop.Wpf.ViewModels.Dialogs;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Dialogs
{
    public partial class QuestionDialogView : ReactiveUserControl<QuestionDialogViewModel>
    {
        public bool? Answer { get; set; } = null;

        public QuestionDialogView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Title, v => v.TitleText.Text);
                this.OneWayBind(ViewModel, vm => vm.Message, v => v.MessageText.Text);
                this.BindCommand(ViewModel, vm => vm.SetResponse, v => v.YesButton, vm => vm.YesResponse);
                this.BindCommand(ViewModel, vm => vm.SetResponse, v => v.NoButton, vm => vm.NoResponse);

            });
        }
    }
}
