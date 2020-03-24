using System;
using System.Windows;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.Dialogs
{
    public partial class QuestionDialogView
    {
        [Reactive] public bool? Answer { get; set; } = null;

        public QuestionDialogView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                //this.OneWayBind(ViewModel, vm => vm.Title, v => v.TitleText.Text);
                //this.OneWayBind(ViewModel, vm => vm.Message, v => v.MessageText.Text);
                //this.BindCommand(ViewModel, vm => vm.SetResponse, v => v.YesButton, vm => vm.YesResponse);
                //this.BindCommand(ViewModel, vm => vm.SetResponse, v => v.NoButton, vm => vm.NoResponse);
                //this.WhenAnyValue(x => x.Answer)
                //    .Where(x => x != null)
                //    .InvokeCommand();

            });
        }

        private void YesButton_OnClick(object sender, RoutedEventArgs e)
        {
            Answer = true;
            ViewModel.CloseCommand.Execute().Subscribe();
        }

        private void NoButton_OnClick(object sender, RoutedEventArgs e)
        {
            Answer = false;
            ViewModel.CloseCommand.Execute().Subscribe();
        }
    }
}
