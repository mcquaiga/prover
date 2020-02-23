﻿using Client.Desktop.Wpf.ViewModels.Dialogs;
using Client.Wpf.Screens;
using ReactiveUI;

namespace Client.Wpf.Views.Dialogs
{
    public partial class TextDialogView : ReactiveDialog<TextDialogViewModel>
    {
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
    }
}
