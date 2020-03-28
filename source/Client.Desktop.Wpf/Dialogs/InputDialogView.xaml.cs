using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

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
