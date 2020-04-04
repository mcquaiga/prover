using ReactiveUI;

namespace Client.Desktop.Wpf.Dialogs
{
    public partial class TextDialogView
    {
        public bool? Answer { get; set; } = null;

        public TextDialogView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                //this.OneWayBind(ViewModel, vm => vm.Title, v => v.TitleText.Text);
                this.OneWayBind(ViewModel, vm => vm.Message, v => v.MessageText.Text);
                this.BindCommand(ViewModel, vm => vm.CloseCommand, v => v.OkButton);
            });
        }

       
    }
}
