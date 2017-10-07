using System;
using System.Reactive.Linq;
using ReactiveUI;

namespace Prover.GUI.Common.Screens.Dialogs
{
    /// <inheritdoc cref="IViewFor{T}" />
    /// <summary>
    ///     Interaction logic for ProgressStatusDialogView.xaml
    /// </summary>
    public partial class ProgressStatusDialogView : IViewFor<ProgressStatusDialogViewModel>
    {
        public ProgressStatusDialogView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                if (ViewModel == null)
                    d(ViewModel = (ProgressStatusDialogViewModel) DataContext);
                else
                {
                    DataContext = ViewModel;
                }


                d(this.WhenAnyValue(x => x.ViewModel.TaskCommand)
                    .SelectMany(x => x.Execute())
                    .Subscribe());
            });
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (ProgressStatusDialogViewModel) value;
        }

        public ProgressStatusDialogViewModel ViewModel { get; set; }
    }
}