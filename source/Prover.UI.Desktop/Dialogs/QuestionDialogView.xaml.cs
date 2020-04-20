using System;
using System.Windows;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.UI.Desktop.Dialogs
{
    public partial class QuestionDialogView
    {
        [Reactive] public bool? Answer { get; set; } = null;

        public QuestionDialogView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                TitleText.Text = ViewModel.Title;
                MessageText.Text = ViewModel.Message;
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
            ViewModel.CancelCommand.Execute().Subscribe();
        }
    }
}
